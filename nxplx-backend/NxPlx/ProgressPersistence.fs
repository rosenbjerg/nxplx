module NxPlx.ProgressPersistence

open System
open System.Globalization
open System.IO
open System.Net
open Red
open Red.Extensions
open Red.Interfaces
open NxPlx.Types
open NxPlx.Utils
open LiteDB
open LiteDB.FSharp.Extensions

let register (router:IRouter) =
    let progressCollection =
        let db = new LiteDatabase(Path.Combine("data", "database", "progress.litedb"))
        db.GetCollection<Progress> "progress"
    
    router.Get("/all", fun (req:Request) (res:Response) ->
        let uid = 0
        let allProgress = progressCollection.findMany (fun e -> e.uid = uid)
        res.SendJson allProgress)
    
    router.Get("/:eid", fun (req:Request) (res:Response) ->
        let eid = req.Context.ExtractUrlParameter "eid" |> Int32.Parse
        let uid = 0
        let id = sprintf "%i-%i" eid uid |> stableHash
        let progress = progressCollection.TryFindById (BsonValue id)
        match progress with
        | Some s -> s.progress.ToString() |> res.SendString 
        | None -> res.SendString "0.0")
    
    router.Post("/:eid", fun (req:Request) (res:Response) ->
        let eid = req.Context.ExtractUrlParameter "eid" |> Int32.Parse
        let jsonProgress = req.ParseBody<JsonProgress>()
        let uid = 0
        let id = sprintf "%i-%i" eid uid |> stableHash
        let progressItem = { id = id; uid=uid; eid=eid; progress = jsonProgress.progress; duration=jsonProgress.duration }
        progressCollection.Upsert progressItem |> ignore
        res.SendStatus HttpStatusCode.OK)
    