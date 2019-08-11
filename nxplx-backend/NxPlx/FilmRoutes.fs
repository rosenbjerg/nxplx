module NxPlx.FilmRoutes

open System
open NxPlx.Types
open NxPlx.TMdbApi
open NxPlx.Indexing
open Red
open Red.Interfaces

let register (router:IRouter) (indexer:Indexer) =
    
    router.Get("/:id", fun (req:Request) (res:Response) ->
        let id = req.Context.ExtractUrlParameter "id" |> Int32.Parse
        let series = Map.find id indexer.film
        res.SendJson series)
        
    router.Get("/:id/details", fun (req:Request) (res:Response) ->
        let details = req.Context.ExtractUrlParameter "id" |> tmdbApi.getFilmDetails
        res.SendString (details, "application/json"))

    router.Get("/:eid/watch", fun (req:Request) (res:Response) ->
        let eid = req.Context.ExtractUrlParameter "eid" |> Int32.Parse
        let film = Map.find eid indexer.filmEntries
        res.SendFile (film.path, "video/mp4", true, 1 * 1024 * 1024))
