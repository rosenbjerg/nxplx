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
                {id=e.id; number=e.episode; description=tmdbEpisodeInfo.overview; thumbnail=tmdbEpisodeInfo.still_path; aired=tmdbEpisodeInfo.air_date})
            {number=number; description=tmdbSeasonInfo.overview; episodes=episodes; poster=tmdbSeasonInfo.poster_path})
        {id=tmdbSeriesInfo.id; name=tmdbSeriesInfo.name; seasons=seasons; aired=tmdbSeriesInfo.first_air_date; rating=tmdbSeriesInfo.vote_average; poster=tmdbSeriesInfo.poster_path; background=tmdbSeriesInfo.backdrop_path})

let indexFilm = Array.Parallel.map (fun f ->
        let info = tmdbApi.findFilm f |> (fun s -> s.results.[0])
        {id=f.id; title=info.title; description=info.overview; released=info.release_date; poster=info.poster_path; background=info.backdrop_path}        
    )

[<EntryPoint>]
let main argv =
    let server = new RedHttpServer()
    
    printfn "Indexing..."
    let directory = "C:\\Users\\Malte\\Documents\\Kode\\boxconverter\\test\\test"
    
    let filmEntries = findFilm directory
    let seriesEntries = findEpisodes directory
    
    
    let film = indexFilm filmEntries
    let series = indexSeries seriesEntries
    
    let filmOverview = Array.map (fun (f:Film) -> {id=f.id; title=f.title; poster=f.poster}) film
    let seriesOverview = Array.map (fun (f:Series) -> {id=f.id; title=f.name; poster=f.poster}) series
    let overview =  Array.concat [ filmOverview; seriesOverview ]
    
    printfn "Done indexing"
        
    server.Get("/", (fun (req:Request) (res:Response) -> res.SendJson overview))
        
    server.Get("/film", (fun (req:Request) (res:Response) -> res.SendJson film))
    server.Get("/film/:id", (fun (req:Request) (res:Response) ->
        let id = Int32.Parse req.Parameters.["id"]
        let film = Array.find (fun (entry:FilmEntry) -> entry.id = id) filmEntries
        res.SendFile film.path))
    
    server.Get("/series", (fun (req:Request) (res:Response) -> res.SendJson series))
    server.Get("/series/:id", (fun (req:Request) (res:Response) ->
        let id = Int32.Parse req.Parameters.["id"]
        let episode = Array.find (fun (entry:EpisodeEntry) -> entry.id = id) seriesEntries
        res.SendFile episode.path))
    
    printfn "Starting NxPlx - the lightweight media server"
    server.RunAsync() |> Async.AwaitTask |> Async.RunSynchronously
    0