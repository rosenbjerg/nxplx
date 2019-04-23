module NxPlx.Server

open Red
open System
open System.IO
open NxPlx.Types
open NxPlx.TMdbApi
open Red

let isEpisode path = path |> seriesRegex.IsMatch
let isFilm path = path |> seriesRegex.IsMatch |> not
let extensionFilter (file:string) = Array.contains (Path.GetExtension file) [| ".mp4"; ".mkv" |]

let findVideoFiles dir = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories) |> Array.where extensionFilter
let findFilm dir = findVideoFiles dir |> Array.where isFilm |> Array.Parallel.map createFilmEntry
let findEpisodes dir = findVideoFiles dir |> Array.where isEpisode |> Array.Parallel.map createEpisodeEntry


let tmdbApi = new TMdbApi(ApiKeyPath("./tmdb-api-key"))
let indexSeries = Array.groupBy (fun (e:EpisodeEntry) -> e.name) >> Array.Parallel.map (fun seriesGroup ->
        let name, seriesEpisodes = seriesGroup
        let tmdbSeriesInfo = tmdbApi.findSeries name |> (fun s -> s.results.[0])
        let seasons = seriesEpisodes |> Array.groupBy (fun e -> e.season) |> Array.Parallel.map (fun seasonGroup ->
            let number, seasonEpisodes = seasonGroup
            let tmdbSeasonInfo = tmdbApi.findSeason tmdbSeriesInfo.id number
            let episodes = seasonEpisodes |> Array.map (fun e ->
                let tmdbEpisodeInfo = tmdbSeasonInfo.episodes.[e.episode - 1]
                tmdbApi.downloadImage "w185" tmdbEpisodeInfo.still_path  |> Async.RunSynchronously |> ignore
                tmdbApi.downloadImage "w500" tmdbEpisodeInfo.still_path  |> Async.RunSynchronously |> ignore
                { id=tmdbEpisodeInfo.id; eid=e.id; number=e.episode; thumbnail=tmdbEpisodeInfo.still_path })
            tmdbApi.downloadImage "w185" tmdbSeasonInfo.poster_path |> Async.RunSynchronously |> ignore
            tmdbApi.downloadImage "w500" tmdbSeasonInfo.poster_path |> Async.RunSynchronously |> ignore
            { number=number; episodes=episodes; poster=tmdbSeasonInfo.poster_path })
        tmdbApi.downloadImage "w185" tmdbSeriesInfo.poster_path |> Async.RunSynchronously |> ignore
        tmdbApi.downloadImage "w500" tmdbSeriesInfo.poster_path |> Async.RunSynchronously |> ignore
        tmdbApi.downloadImage "original" tmdbSeriesInfo.backdrop_path |> Async.RunSynchronously |> ignore
        { id=tmdbSeriesInfo.id; name=tmdbSeriesInfo.name; seasons=seasons; poster=tmdbSeriesInfo.poster_path })

let indexFilm = Array.Parallel.map (fun f ->
        let info = tmdbApi.findFilm f |> (fun s -> s.results.[0])
        tmdbApi.downloadImage "w185" info.poster_path |> Async.RunSynchronously |> ignore
        tmdbApi.downloadImage "w500" info.poster_path |> Async.RunSynchronously |> ignore
        tmdbApi.downloadImage "original" info.backdrop_path |> Async.RunSynchronously |> ignore
        { id=info.id; eid=f.id; title=info.title; poster=info.poster_path }        
    )

[<EntryPoint>]
let main argv =    
    printfn "Indexing..."
    let directory = "C:\\Users\\Malte\\Documents\\Kode\\boxconverter\\test\\test"
    
    let filmEntries = findFilm directory
    let seriesEntries = findEpisodes directory
    
    let film = indexFilm filmEntries
    let series = indexSeries seriesEntries
    
    let filmOverview = Array.map (fun (f:Film) -> {id=f.id; title=f.title; poster=f.poster; kind="film"}) film
    let seriesOverview = Array.map (fun (f:Series) -> {id=f.id; title=f.name; poster=f.poster; kind="series"}) series
    let overview =  Array.concat [ filmOverview; seriesOverview ]
    
    printfn "Done indexing"
        
    let server = new RedHttpServer (5990, "public")
    server.Get("/api/overview", (fun (req:Request) (res:Response) -> res.SendJson overview))
    server.Get("/api/posters/*", Red.Utils.SendFiles "public/posters")
            
    server.Get("/api/film/:id", (fun (req:Request) (res:Response) ->
        let id = Int32.Parse (req.Context.ExtractUrlParameter "id")
        let series = Array.find (fun (entry:Film) -> entry.id = id) film
        res.SendJson series))
            
    server.Get("/api/film/:id/details", (fun (req:Request) (res:Response) ->
        let details = tmdbApi.getFilmDetails (req.Context.ExtractUrlParameter "id")
        res.SendString (details, "application/json")))
    
    server.Get("/api/film/:eid/watch", (fun (req:Request) (res:Response) ->
        let id = Int32.Parse (req.Context.ExtractUrlParameter "eid")
        let film = Array.find (fun (entry:FilmEntry) -> entry.id = id) filmEntries
        res.SendFile film.path))
    
    
    server.Get("/api/series/:id", (fun (req:Request) (res:Response) ->
        let id = Int32.Parse (req.Context.ExtractUrlParameter "id")
        let series = Array.find (fun (entry:Series) -> entry.id = id) series
        res.SendJson series))
    
    server.Get("/api/series/:id/details", (fun (req:Request) (res:Response) ->
        let details = tmdbApi.getSeriesDetails (req.Context.ExtractUrlParameter "id")
        res.SendString (details, "application/json")))
    
    server.Get("/api/series/:eid/watch", (fun (req:Request) (res:Response) ->
        let id = Int32.Parse (req.Context.ExtractUrlParameter "eid")
        let episode = Array.find (fun (entry:EpisodeEntry) -> entry.id = id) seriesEntries
        res.SendFile episode.path))
    
    printfn "Starting NxPlx - the lightweight media server"
    server.RunAsync() |> Async.AwaitTask |> Async.RunSynchronously
    0