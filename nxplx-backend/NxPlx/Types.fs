module NxPlx.Types

open System
open System.IO
open System.Text.RegularExpressions

let seriesRegex = new Regex ("^(?<name>.+?)??[ -]*([Ss](?<season>\d{1,3}))? ?[Ee](?<episode>\d{1,3})", RegexOptions.Compiled)
let filmRegex = new Regex ("^(?<title>.+?)??\(?(?<year>\d{4})\)?[ .]?", RegexOptions.Compiled)
let whitespaceRegex = new Regex ("[\s.-]", RegexOptions.Compiled)
let titleCleanup title =
    let whitespaceCleared = whitespaceRegex.Replace (title, " ")
    whitespaceCleared.Trim ([|' '; '.'; '-'|])



type EpisodeEntry = {id:int; name:string; season:int; episode:int; path:string; size:int64}
let createEpisodeEntry path =
    let id = path.GetHashCode()
    let size = (new System.IO.FileInfo(path)).Length
    let seriesMatch = Path.GetFileNameWithoutExtension path |> seriesRegex.Match
    let nameGroup = seriesMatch.Groups.["name"];
    let seasonGroup = seriesMatch.Groups.["season"];
    let episodeGroup = seriesMatch.Groups.["episode"];
    
    let name = (if nameGroup.Success then nameGroup.Value else "") |> titleCleanup
    let season = if seasonGroup.Success then Int32.Parse seasonGroup.Value else 1
    let episode = if episodeGroup.Success then Int32.Parse episodeGroup.Value else 1
    {id=id; name=name; season=season; episode=episode; path=path; size=size}
  
type FilmEntry = {id:int; title:string; year:int; path:string; size:int64}
let createFilmEntry path =
    let id = path.GetHashCode()
    let filesize = (new System.IO.FileInfo(path)).Length
    let filmMatch = Path.GetFileNameWithoutExtension path |> filmRegex.Match
    let titleGroup = filmMatch.Groups.["title"];
    let yearGroup = filmMatch.Groups.["year"];
    
    let title = (if titleGroup.Success then titleGroup.Value else "") |> titleCleanup
    let year = if yearGroup.Success then Int32.Parse yearGroup.Value else -1
    {id=id; title=title; year=year; path=path; size=filesize}


type Episode = {id:int; eid:int; number:int; description:string; thumbnail:string; aired:DateTime}
type Season = {number:int; description:string; episodes:Episode array; poster:string}
type Series = {id:int; name:string; seasons:Season array; aired:DateTime; rating:float32; poster:string; background:string}
type Film = {id:int; eid:int; title:string; description:string; released:DateTime; poster:string; background:string}
type Info = {id:int; title:string; poster:string; kind:string}