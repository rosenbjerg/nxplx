module NxPlx.Server

open NxPlx
open Red
open System.Net
    
[<EntryPoint>]
let main argv =
//    System.Globalization.CultureInfo.CurrentCulture <- CultureInfo.InvariantCulture
//    System.Globalization.CultureInfo.DefaultThreadCurrentCulture <- CultureInfo.InvariantCulture
//    System.Threading.Thread.CurrentThread.CurrentCulture <- CultureInfo.InvariantCulture
    
    let indexer = new NxPlx.Indexing.Indexer "C:\\NxPlx test data"
    
    printfn "Indexing..."    
    indexer.index
    printfn "Done indexing"
        
    let server = new RedHttpServer (5990, "public")
    
    server.Get("/api/overview", fun (req:Request) (res:Response) -> res.SendJson indexer.entries)
    
    server.Get("/api/posters/*", Red.Utils.SendFiles "data/posters")
    
    server.Post("/api/scan", fun (req:Request) (res:Response) ->
        indexer.index
        res.SendStatus HttpStatusCode.OK)

    FilmRoutes.register (server.CreateRouter "/api/film") indexer
    SeriesRoutes.register (server.CreateRouter "/api/series") indexer
    EpisodeRoutes.register (server.CreateRouter "/api/episode") indexer
    
    SubtitlePersistence.register (server.CreateRouter "/api/subtitle") indexer
    ProgressPersistence.register (server.CreateRouter "/api/progress")
    
    printfn "Starting NxPlx - the lightweight media server"
    server.RunAsync() |> Async.AwaitTask |> Async.RunSynchronously
    0