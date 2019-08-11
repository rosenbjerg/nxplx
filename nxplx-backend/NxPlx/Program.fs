module NxPlx.Server

open NxPlx
open NxPlx.Indexing
open Red
open Red.Extensions
open System
open System.Globalization
open System.Net
open System.Threading.Tasks

let handlerMapper = Array.map (fun e -> new Func<Request, Response, Task<HandlerType>>(e))

type Red.RedHttpServer with
    member __.get (route:string) ([<ParamArray>] handlers:(Request -> Response -> Task<HandlerType>) array) =
        __.Get(route, handlerMapper handlers)
    
[<EntryPoint>]
let main argv =
//    System.Globalization.CultureInfo.CurrentCulture <- CultureInfo.InvariantCulture
//    System.Globalization.CultureInfo.DefaultThreadCurrentCulture <- CultureInfo.InvariantCulture
//    System.Threading.Thread.CurrentThread.CurrentCulture <- CultureInfo.InvariantCulture
    
    let indexer = new NxPlx.Indexing.Indexer "C:\\Users\\Malte\\Documents\\Kode\\boxconverter\\test\\test"
    
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