module NxPlx.TMdbApi

open FSharp.Control.Tasks
open NxPlx.Types
open System.Net.Http
open Newtonsoft.Json
open System
open System.IO
open System.Web
open LiteDB
open LiteDB.FSharp.Extensions

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

[<CLIMutable>]
type CachedTMdbSearchRequest = {id:string; content:string; expires:DateTime; }

     
let fetchWeb (httpClient:HttpClient) (url:string) = async {
    let! response = httpClient.GetAsync url |> Async.AwaitTask
    let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
    return content
}    
    
let fetch (cache:LiteCollection<CachedTMdbSearchRequest>) (httpClient:HttpClient) (validTimeSpan:TimeSpan) (url:string) =
    let expiryDate = DateTime.UtcNow.Add validTimeSpan
    
    let bsonId = BsonValue url;
    let cachedOption = cache.TryFindById bsonId
    
    match cachedOption with
    | Some c ->
        if c.expires > DateTime.UtcNow  then
            c.content
        else
            let content = fetchWeb httpClient url |> Async.RunSynchronously
            cache.Update {id=url; content=content; expires=expiryDate} |> ignore
            content
    | None ->
        let content = fetchWeb httpClient url |> Async.RunSynchronously
        cache.Insert {id=url; content=content; expires=expiryDate} |> ignore
        content

type TMdbApiKey = ApiKeyValue of string | ApiKeyPath of string

type TMdbApi(apiKey:TMdbApiKey) =
    
    let key = match apiKey with
              | ApiKeyValue k -> k
              | ApiKeyPath kp -> System.IO.File.ReadAllText kp
    
    let db =
        let dir = Path.Combine("data", "database");
        Directory.CreateDirectory dir |> ignore
        let db = new LiteDatabase(Path.Combine(dir, "tmdb-cache.litedb"))
        db.GetCollection<CachedTMdbSearchRequest> "requests"
       
    
    let client =
        let client = new HttpClient()
        client.DefaultRequestHeaders.Add("User-Agent", "NxPlx")
        client
       
    let _fetchAndDeserialize validTimeSpan url : 'a =
        fetch db client validTimeSpan url |> JsonConvert.DeserializeObject<'a>
        
    let _fetch validTimeSpan url =
        fetch db client validTimeSpan url
        
    member __.downloadImage (size:string) (posterUrl:string) =
        let dir = Path.Combine ("data", "posters")
        let run = task {
            Directory.CreateDirectory dir |> ignore
            let posterName = Path.GetFileNameWithoutExtension posterUrl
            let outputPath = Path.Combine (dir, (sprintf "%s-%s.jpg" posterName size))
            if File.Exists outputPath then return outputPath
            else 
                let! response = client.GetAsync((sprintf "http://image.tmdb.org/t/p/%s%s?api_key=%s" size posterUrl key))
                use! inputStream = response.Content.ReadAsStreamAsync()
                use outputStream = File.OpenWrite outputPath
                do! inputStream.CopyToAsync outputStream
                return outputPath
        }
        run |> ignore
    member __.getFilmDetails id =    
        _fetch (TimeSpan.FromDays 500.0) (sprintf "https://api.themoviedb.org/3/movie/%s?api_key=%s" id key)
    member __.getSeriesDetails id =    
        _fetch (TimeSpan.FromDays 50.0) (sprintf "https://api.themoviedb.org/3/tv/%s?api_key=%s" id key)
    member __.getEpisodeDetails id season episode =    
        _fetch (TimeSpan.FromDays 500.0) (sprintf "https://api.themoviedb.org/3/tv/%s/season/%s/episode/%s?api_key=%s" id season episode key)
        
    
    member __.findFilm (film:FilmEntry) : TMdbFilmSearch  =
        let year = match film.year with | -1 -> "" | _ -> sprintf "&year=%i" film.year        
        _fetchAndDeserialize (TimeSpan.FromDays 500.0) (sprintf "https://api.themoviedb.org/3/search/movie?api_key=%s&query=%s%s" key (HttpUtility.UrlEncode film.title) year)
    member __.findSeries (name:string) : TMdbSeriesSearch =
        _fetchAndDeserialize (TimeSpan.FromDays 500.0) (sprintf "https://api.themoviedb.org/3/search/tv?api_key=%s&query=%s" key (HttpUtility.UrlEncode name))
    member __.findSeason  id season : TMdbSeasonSearch   =
        _fetchAndDeserialize (TimeSpan.FromDays 100.0) (sprintf "https://api.themoviedb.org/3/tv/%i/season/%i?api_key=%s" id season key)

let tmdbApi = new TMdbApi(ApiKeyPath(Path.Combine("data", "tmdb-api-key")))