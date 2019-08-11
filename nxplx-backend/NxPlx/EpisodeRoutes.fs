module NxPlx.EpisodeRoutes

open System
open NxPlx.Types
open NxPlx.TMdbApi
open NxPlx.Indexing
open Red
open Red.Interfaces

type EpisodeInfo = {id:int; eid:int; title:string; season:int; number:int; subtitles:string array; backdrop:string}

let register (router:IRouter) (indexer:Indexer) =        
        
    router.Get("/:id", fun (req:Request) (res:Response) ->
        let id = req.Context.ExtractUrlParameter "id" |> Int32.Parse
        let episode = Map.find id indexer.episodes
        let series = Map.find episode.series indexer.series
        res.SendJson {id=episode.id; eid=episode.eid; title=series.name; season=episode.season; number=episode.number;
                      subtitles=episode.subtitles; backdrop=episode.thumbnail})
          
    router.Get("/:eid/watch", fun (req:Request) (res:Response) ->
        let eid = req.Context.ExtractUrlParameter "eid" |> Int32.Parse
        let episode = Map.find eid indexer.episodeEntries
        res.SendFile (episode.path, "video/mp4", true, 1 * 1024 * 1024))
