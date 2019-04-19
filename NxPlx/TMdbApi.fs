module NxPlx.TMdbApi

open NxPlx.Types
open System.Net.Http
open Newtonsoft.Json
open System
open System
open System.IO
open System.Web
open LiteDB.FSharp
open LiteDB.FSharp.Extensions
open LiteDB

type TMdbFilmResult = {vote_count:int; id:int; video:bool; vote_average:float32; title:string; popularity:float32;
                       poster_path:string; original_language:string; original_title:string; genre_ids: int array;
                       backdrop_path:string; adult:bool; overview:string; release_date:DateTime}

type TMdbSeriesResult = {original_name:string; id:int; name:string; vote_count:int; vote_average:float32;
                         poster_path:string; first_air_date:DateTime; popularity:float32; genre_ids:int array;
                         original_language:string; backdrop_path:string; overview:string; origin_country:string array}


type TMdbEpisodeCrewResult = {id:int; credit_id:string; name:string; department:string; job:string; profile_path:string}
type TMdbEpisodeGuestStarResult = {id:int; name:string; credit_id:string; character:string;
                                   order:int; gender:int; profile_path:string}
type TMdbEpisodeResult = {air_date:DateTime; episode_number:int; id:int; name:string; overview:string;
                          production_code:string; season_number:int; show_id:int; still_path:string;
                          vote_average:float32; vote_count:int; crew:TMdbEpisodeCrewResult array;
                          guest_stars:TMdbEpisodeGuestStarResult array}


type TMdbFilmSearch = {page:int; total_results:int; total_pages:int; results: TMdbFilmResult array}
type TMdbSeriesSearch = {page:int; total_results:int; total_pages:int; results: TMdbSeriesResult array}
type TMdbSeasonSearch = {_id:string; air_date:DateTime; episodes:TMdbEpisodeResult array; name:string;
                          overview:string; id:int; poster_path:string; season_number:int}

type CachedTMdbSearchRequest<'a> =
    val mutable id:string; val mutable content:'a; val mutable requested:DateTime; 
    new(id, content) = {id=id; content=content; requested=DateTime.UtcNow}
    new() = {id=""; content=Unchecked.defaultof<'a>; requested=DateTime.UtcNow}
    
let fetchCached (db:LiteDatabase) (requestType:string) (url:string) =        
    let expiryDate = DateTime.UtcNow.Subtract (TimeSpan.FromDays 7.0)
    let cache = db.GetCollection<CachedTMdbSearchRequest<'a>>(requestType)
    let cached = cache.TryFind (Query.EQ("id", (BsonValue url)))
    match cached with
    | Some c ->
        if c.requested < expiryDate then
            cache.delete (fun c -> c.id = url) |> ignore
            None
        else Some(c.content)
    | None -> None

     
let fetchWeb (httpClient:HttpClient) (url:string) = async {
    let! response = httpClient.GetAsync url |> Async.AwaitTask
    let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
    return content |> JsonConvert.DeserializeObject<'a>
}    
    
let fetch (db:LiteDatabase) (httpClient:HttpClient) (requestType:string) (url:string) =
    let cachedOption = fetchCached db requestType url
    match cachedOption with
    | Some c -> c
    | None ->
        let cache = db.GetCollection<CachedTMdbSearchRequest<'a>>(requestType)
        let content = fetchWeb httpClient url |> Async.RunSynchronously
        let doc = new CachedTMdbSearchRequest<'a>(url, content)
        cache.Insert doc |> ignore
        content

type TMdbApiKey = ApiKeyValue of string | ApiKeyPath of string
type TMdbApi(apiKey:TMdbApiKey) =
    
    let key = match apiKey with
              | ApiKeyValue k -> k
              | ApiKeyPath kp -> System.IO.File.ReadAllText kp
    
       
    let _fetch requestType url :'a =
        let client = new HttpClient()
        client.DefaultRequestHeaders.Add("User-Agent", "NxPlx")
        Directory.CreateDirectory "database" |> ignore
        let db = new LiteDatabase(Path.Combine("database", "tmdb-cache.sqlite"), FSharpBsonMapper())
        
        fetch db client requestType url
        
    
    let _findFilm (film:FilmEntry) : TMdbFilmSearch  =
        let year = match film.year with | -1 -> "" | _ -> sprintf "&year=%i" film.year        
        _fetch "film" (sprintf "https://api.themoviedb.org/3/search/movie?api_key=%s&query=%s%s" key (HttpUtility.UrlEncode film.title) year)
    let _findSeries (name:string) : TMdbSeriesSearch =
        _fetch "series" (sprintf "https://api.themoviedb.org/3/search/tv?api_key=%s&query=%s" key (HttpUtility.UrlEncode name))
    let _findSeason id season : TMdbSeasonSearch   =
        _fetch "season" (sprintf "https://api.themoviedb.org/3/tv/%i/season/%i?api_key=%s" id season key)
        
    member this.findFilm film = _findFilm film
    member this.findSeries name = _findSeries name
    member this.findSeason id season = _findSeason id season
        