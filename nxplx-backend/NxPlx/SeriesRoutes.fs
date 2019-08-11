module NxPlx.SeriesRoutes

open System
open NxPlx.Types
open NxPlx.TMdbApi
open NxPlx.Indexing
open Red
open Red.Interfaces

let register (router:IRouter) (indexer:Indexer) =        
        
    router.Get("/:id", fun (req:Request) (res:Response) ->
        let id = req.Context.ExtractUrlParameter "id" |> Int32.Parse
        let series = Map.find id indexer.series
        res.SendJson series)
    
    router.Get("/episode/:id", fun (req:Request) (res:Response) ->
        let id = req.Context.ExtractUrlParameter "id" |> Int32.Parse
        let episode = Map.find id indexer.episodes
        res.SendJson episode)

    
    router.Get("/:id/details", fun (req:Request) (res:Response) ->
        let details = tmdbApi.getSeriesDetails (req.Context.ExtractUrlParameter "id")
        res.SendString (details, "application/json"))
    
    router.Get("/season/:id/details", fun (req:Request) (res:Response) ->
        let details = tmdbApi.getSeriesDetails (req.Context.ExtractUrlParameter "id")
        res.SendString (details, "application/json"))
        