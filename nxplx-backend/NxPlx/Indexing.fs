module NxPlx.Indexing

open LiteDB
open LiteDB.FSharp.Extensions
open System
open System.IO
open NxPlx.Types
open NxPlx.Utils
open NxPlx.TMdbApi
open NxPlx.SrtConverter



let filmCollection =
    let db = new LiteDB.LiteDatabase(Path.Combine("database", "film.litedb"))
    db.GetCollection<FilmEntry> "film"
    
let seriesCollection =
    let db = new LiteDB.LiteDatabase(Path.Combine("database", "series.litedb"))
    db.GetCollection<Episode> "series"


let extensionFilter (file:string) = Array.contains (Path.GetExtension file) [| ".mp4"; ".mkv" |]

let existanceFilter (kind:string) (file:string) =
    let hash = stableHash file |> BsonValue
    match kind with
    | "film" -> hash |> filmCollection.TryFindById = None |> not
    | "series" -> hash |> filmCollection.TryFindById = None |> not
    | _ -> failwith "invalid kind parameter value"


let getSubtitleLanguage (path:string) =
    let filename = Path.GetFileNameWithoutExtension path
    let index = filename.LastIndexOf '.'
    filename.Substring (index + 1)
    
let findSubtitles (path:string) =
    let filename = Path.GetFileNameWithoutExtension path
    let srtFiles = Directory.GetFiles((Path.GetDirectoryName path), (sprintf "%s.*.srt" filename))
    
    srtFiles |> Array.map (fun srt ->
        let vttFile = srt.Replace(".srt", ".vtt")
        if File.Exists vttFile then
            getSubtitleLanguage vttFile
        else
            srt2vtt srt vttFile 0.0
            getSubtitleLanguage vttFile)
    
    
let createEpisodeEntry path =
    let id = stableHash path
    let filesize = (new System.IO.FileInfo(path)).Length
    let creationTime = File.GetCreationTimeUtc path
    let seriesMatch = Path.GetFileNameWithoutExtension path |> seriesRegex.Match
    let nameGroup = seriesMatch.Groups.["name"];
    let seasonGroup = seriesMatch.Groups.["season"];
    let episodeGroup = seriesMatch.Groups.["episode"];
    
    let subtitles = findSubtitles path
    let name = (if nameGroup.Success then nameGroup.Value else "") |> titleCleanup
    let season = if seasonGroup.Success then Int32.Parse seasonGroup.Value else 1
    let episode = if episodeGroup.Success then Int32.Parse episodeGroup.Value else 1
    {id=id; name=name; season=season; episode=episode; subtitles=subtitles; creationTime=creationTime; path=path; size=filesize}
    
let createFilmEntry path =
    let id = stableHash path
    let filesize = (new System.IO.FileInfo(path)).Length
    let creationTime = File.GetCreationTimeUtc path
    let filmMatch = Path.GetFileNameWithoutExtension path |> filmRegex.Match
    let titleGroup = filmMatch.Groups.["title"];
    let yearGroup = filmMatch.Groups.["year"];
    
    let subtitles = findSubtitles path
    let title = if titleGroup.Success then titleGroup.Value |> titleCleanup else ""
    let year = if yearGroup.Success then Int32.Parse yearGroup.Value else -1
    {id=id; title=title; year=year; subtitles=subtitles; creationTime=creationTime; path=path; size=filesize}
    

let findVideoFiles dir = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories) |> Array.where extensionFilter
let findFilm = findVideoFiles >> Array.where isFilm >> Array.Parallel.map createFilmEntry
let findEpisodes = findVideoFiles >> Array.where isEpisode >> Array.Parallel.map createEpisodeEntry

let indexSeries = Array.groupBy (fun (e:EpisodeEntry) -> e.name) >> Array.Parallel.map (fun seriesGroup ->
        let name, seriesEpisodes = seriesGroup
        let seriesInfo = tmdbApi.findSeries name |> (fun s -> s.results.[0])
        let seasons = seriesEpisodes |> Array.groupBy (fun e -> e.season) |> Array.Parallel.map (fun seasonGroup ->
            let number, seasonEpisodes = seasonGroup
            let seasonInfo = tmdbApi.findSeason seriesInfo.id number
            let episodes = seasonEpisodes |> Array.map (fun e ->
                let episodeInfo = seasonInfo.episodes.[e.episode - 1]
                tmdbApi.downloadImage "w185" episodeInfo.still_path  |> ignore
                tmdbApi.downloadImage "w1280" episodeInfo.still_path  |> ignore
                { id=episodeInfo.id; eid=e.id; series=seriesInfo.id; season=e.season; subtitles=e.subtitles; number=e.episode; thumbnail=Path.GetFileNameWithoutExtension episodeInfo.still_path })
            tmdbApi.downloadImage "w154" seasonInfo.poster_path |> ignore
            tmdbApi.downloadImage "w342" seasonInfo.poster_path |> ignore
            { number=number; episodes=episodes; poster=Path.GetFileNameWithoutExtension seasonInfo.poster_path })
        tmdbApi.downloadImage "w154" seriesInfo.poster_path |> ignore
        tmdbApi.downloadImage "w342" seriesInfo.poster_path |> ignore
        tmdbApi.downloadImage "w1280" seriesInfo.backdrop_path |> ignore
        { id=seriesInfo.id; name=seriesInfo.name; seasons=seasons; poster=Path.GetFileNameWithoutExtension seriesInfo.poster_path; backdrop=Path.GetFileNameWithoutExtension seriesInfo.backdrop_path })

let indexFilm = Array.Parallel.map (fun (f:FilmEntry) ->
        let info = tmdbApi.findFilm f |> (fun s -> s.results.[0])
        tmdbApi.downloadImage "w154" info.poster_path
        tmdbApi.downloadImage "w342" info.poster_path
        tmdbApi.downloadImage "w1280" info.backdrop_path
        { id=info.id; eid=f.id; title=info.title; subtitles=f.subtitles; poster=Path.GetFileNameWithoutExtension info.poster_path; backdrop=Path.GetFileNameWithoutExtension info.backdrop_path })

type Indexer (directory:string) =
    let mutable _filmEntries = Map.empty
    let mutable _episodeEntries = Map.empty
    let mutable _overview = Array.empty
    
    let mutable _film = Map.empty
    let mutable _series = Map.empty
    let mutable _episodes = Map.empty
    
    
    member __.index =
        let filmEntries = findFilm directory
        let episodeEntries = findEpisodes directory
        
        let film = indexFilm filmEntries
        let series = indexSeries episodeEntries
        
        _filmEntries <- filmEntries |> Array.map (fun e -> e.id, e) |> Map.ofArray
        _episodeEntries <- episodeEntries |> Array.map (fun e -> e.id, e) |> Map.ofArray
        
        
        _film <- film  |> Array.map (fun e -> e.id, e) |> Map.ofArray
        _series <- series
                   |> Array.map (fun e -> e.id, e)
                   |> Map.ofArray
        _episodes <- series
                   |> Array.collect (fun s -> s.seasons)
                   |> Array.collect (fun s -> s.episodes)
                   |> Array.map (fun e -> e.id, e)
                   |> Map.ofArray
                
        _overview <- Array.concat [
            Array.map (fun (f:Film) -> {id=f.id; title=f.title; poster=f.poster; kind="film"}) film
            Array.map (fun (f:Series) -> {id=f.id; title=f.name; poster=f.poster; kind="series"}) series
        ]
        
    member __.film = _film
    member __.filmEntries = _filmEntries
    member __.series = _series
    member __.episodes = _episodes
    member __.episodeEntries = _episodeEntries
    member __.entries = _overview
