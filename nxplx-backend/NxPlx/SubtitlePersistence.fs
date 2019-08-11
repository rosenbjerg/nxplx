module NxPlx.SubtitlePersistence

open FSharp.Control.Tasks
open System
open System.IO
open System.Net
open Red
open Red.Extensions
open Red.Interfaces
open NxPlx.Types
open NxPlx.Utils
open NxPlx.Indexing
open LiteDB
open LiteDB.FSharp.Extensions

let register (router:IRouter) (indexer:Indexer) =
    let subtitleCollection =
        let db = new LiteDatabase(Path.Combine("data", "database", "subtitle-settings.litedb"))
        db.GetCollection<SubtitlePreference> "subtitles"
    
    let getSubtitleUrl (videoPath:string) lang =
        let dir = Path.GetDirectoryName videoPath
        let name = Path.GetFileNameWithoutExtension videoPath
        Path.Combine (dir, (sprintf "%s.%s.vtt" name lang))
    
    router.Get("/:eid", fun (req:Request) (res:Response) ->
        let eid = req.Context.ExtractUrlParameter "eid" |> Int32.Parse
        let uid = 0
        let id = sprintf "%i-%i" eid uid |> stableHash
        let settings = subtitleCollection.TryFindById (BsonValue id)
        match settings with
        | Some s -> res.SendString s.language
        | None -> res.SendString "none")
    
    router.Post("/:eid", fun (req:Request) (res:Response) -> 
        let eid = req.Context.ExtractUrlParameter "eid" |> Int32.Parse
        let lang = req.ParseBody<JsonValue<string>>()
        let uid = 0
        let id = sprintf "%i-%i" eid uid |> stableHash
        let subtitleSettings = {id = id; language = lang.value}
        subtitleCollection.Upsert subtitleSettings |> ignore
        res.SendStatus HttpStatusCode.OK)
        
    
    router.Get("/film/:eid/:lang", fun (req:Request) (res:Response) ->
        let eid = req.Context.ExtractUrlParameter "eid" |> Int32.Parse
        let lang = req.Context.ExtractUrlParameter "lang"
        let film = Map.find eid indexer.filmEntries
        lang |> getSubtitleUrl film.path |> res.SendFile)
    
    router.Get("/episode/:eid/:lang", fun (req:Request) (res:Response) ->
        let eid = req.Context.ExtractUrlParameter "eid" |> Int32.Parse
        let lang = req.Context.ExtractUrlParameter "lang"
        let episode = Map.find eid indexer.episodeEntries
        lang |> getSubtitleUrl episode.path |> res.SendFile)