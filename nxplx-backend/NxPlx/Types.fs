module NxPlx.Types

open System



type EpisodeEntry = { id:int; name:string; season:int; episode:int; subtitles:string array; creationTime:DateTime; path:string; size:int64 }
type FilmEntry = { id:int; title:string; year:int; subtitles:string array; creationTime:DateTime; path:string; size:int64}

type Episode = { id:int; eid:int; number:int; series:int; season:int; subtitles:string array; thumbnail:string }
type Season = { number:int; episodes:Episode array; poster:string }
type Series = { id:int; name:string; seasons:Season array; poster:string; backdrop:string }

type Film = { id:int; eid:int; title:string; subtitles:string array; poster:string; backdrop:string }

type Entry = { id:int; title:string; poster:string; kind:string }

type JsonValue<'a> = { value:'a }

type JsonProgress = { progress: float32; duration: float32 }

[<CLIMutable>]
type User = { id:int; username:string; password:string }
[<CLIMutable>]
type SubtitlePreference = { id:int; language:string }
[<CLIMutable>]
type Progress = { id:int; uid:int; eid:int; progress:float32; duration: float32 }