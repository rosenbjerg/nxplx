module NxPlx.Types

open System
open System.IO
open System.Text.RegularExpressions
open NxPlx.SrtConverter

let seriesRegex = new Regex ("^(?<name>.+?)??[ -]*([Ss](?<season>\d{1,3}))? ?[Ee](?<episode>\d{1,3})", RegexOptions.Compiled)
let filmRegex = new Regex ("^(?<title>.+?)??\(?(?<year>\d{4})\)?[ .]?", RegexOptions.Compiled)
let whitespaceRegex = new Regex ("[\s.-]", RegexOptions.Compiled)
let titleCleanup title =
    let whitespaceCleared = whitespaceRegex.Replace (title, " ")
    whitespaceCleared.Trim ([|' '; '.'; '-'|])

    
let hashPath (path:string) =
    path |> Seq.map int |> Seq.reduce (fun (hash:int) (c:int) -> (((hash <<< 5) - hash) + c) ||| 0)


let getSubtitleLanguage (path:string) =
    let filename = Path.GetFileNameWithoutExtension path
    let index = filename.LastIndexOf '.'
    filename.Substring (index + 1)
    
let findSubtitles (path:string) =
    let filename = Path.GetFileNameWithoutExtension path
    let srtFiles = Directory.GetFiles((Path.GetDirectoryName path), (sprintf "%s.*.srt" filename))
    
    let subtitles = srtFiles |> Array.map (fun srt ->
        let vttFile = srt.Replace(".srt", ".vtt")
        if File.Exists vttFile then
            getSubtitleLanguage vttFile
        else
            srt2vtt srt vttFile 0.0
            getSubtitleLanguage vttFile)
    subtitles

type EpisodeEntry = { id:int; name:string; season:int; episode:int; subtitles:string array; path:string; size:int64 }
let createEpisodeEntry path =
    let id = hashPath path
    let size = (new System.IO.FileInfo(path)).Length
    let seriesMatch = Path.GetFileNameWithoutExtension path |> seriesRegex.Match
    let nameGroup = seriesMatch.Groups.["name"];
    let seasonGroup = seriesMatch.Groups.["season"];
    let episodeGroup = seriesMatch.Groups.["episode"];
    
    let subtitles = findSubtitles path
    let name = (if nameGroup.Success then nameGroup.Value else "") |> titleCleanup
    let season = if seasonGroup.Success then Int32.Parse seasonGroup.Value else 1
    let episode = if episodeGroup.Success then Int32.Parse episodeGroup.Value else 1
    {id=id; name=name; season=season; episode=episode; subtitles=subtitles; path=path; size=size}
  
type FilmEntry = { id:int; title:string; year:int; subtitles:string array; path:string; size:int64 }
let createFilmEntry path =
    let id = hashPath path
    let filesize = (new System.IO.FileInfo(path)).Length
    let filmMatch = Path.GetFileNameWithoutExtension path |> filmRegex.Match
    let titleGroup = filmMatch.Groups.["title"];
    let yearGroup = filmMatch.Groups.["year"];
    
    let subtitles = findSubtitles path
    let title = (if titleGroup.Success then titleGroup.Value else "") |> titleCleanup
    let year = if yearGroup.Success then Int32.Parse yearGroup.Value else -1
    {id=id; title=title; year=year; subtitles=subtitles; path=path; size=filesize}


type Episode = { id:int; eid:int; number:int; subtitles:string array; thumbnail:string }
type Season = { number:int; episodes:Episode array; poster:string }
type Series = { id:int; name:string; seasons:Season array; poster:string }
type Film = { id:int; eid:int; title:string; subtitles:string array; poster:string }
type Info = { id:int; title:string; poster:string; kind:string }