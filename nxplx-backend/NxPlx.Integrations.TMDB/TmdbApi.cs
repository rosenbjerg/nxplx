using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NxPlx.Models.TMDbApi.Movie;
using NxPlx.Models.TMDbApi.Search;
using NxPlx.Models.TMDbApi.Tv;
using NxPlx.Models.TMDbApi.TvSeason;
using NxPlx.Services.Caching;

namespace NxPlx.Integrations.TMDBApi
{
    public class TmdbApi
    {
        private const string ImageFolder = "images";
        
        private string _key;
        private ICachingService _cachingService;
        private HttpClient _client = new HttpClient
        {
            DefaultRequestHeaders =
            {
                {"User-Agent", "NxPlx"}
            }
        };


        public TmdbApi(string apiKey, ICachingService cachingService)
        {
            _key = apiKey;
            _cachingService = cachingService;
            Directory.CreateDirectory(Path.Combine("data", ImageFolder));
        }
        

        public async Task<SearchResult<MovieResult>> SearchMovies(string title, int year = 0)
        {
            var encodedTitle = HttpUtility.UrlEncode(title);
            var encodedYear = year == 0 ? "" : $"&year={year}";
            var url = $"https://api.themoviedb.org/3/search/movie?api_key={_key}&query={encodedTitle}{encodedYear}";

            var content = await Fetch(url);
            return JsonConvert.DeserializeObject<SearchResult<MovieResult>>(content);
        }
        
        public async Task<SearchResult<TVShowResult>> SearchTvShows(string name)
        {
            var encodedTitle = HttpUtility.UrlEncode(name);
            var url = $"https://api.themoviedb.org/3/search/tv?api_key={_key}&query={encodedTitle}";

            var content = await Fetch(url);
            return JsonConvert.DeserializeObject<SearchResult<TVShowResult>>(content);
        }

        public async Task<MovieDetails> FetchMovieDetails(int id)
        {
            var url = $"https://api.themoviedb.org/3/movie/{id}?api_key={_key}";
            
            var content = await Fetch(url);
            return JsonConvert.DeserializeObject<MovieDetails>(content);
        }
        
        public async Task<TvDetails> FetchTvDetails(int id)
        {
            var url = $"https://api.themoviedb.org/3/tv/{id}?api_key={_key}";
            
            var content = await Fetch(url);
            return JsonConvert.DeserializeObject<TvDetails>(content);
        }
        
        public async Task<TvSeasonDetails> FetchTvSeasonDetails(int id, int season)
        {
            var url = $"https://api.themoviedb.org/3/tv/{id}/season/{season}?api_key={_key}";

            var content = await Fetch(url);
            return JsonConvert.DeserializeObject<TvSeasonDetails>(content);
        }
        
        public async Task<string> DownloadImage(string size, string imageUrl)
        {
            var imageName = Path.GetFileNameWithoutExtension(imageUrl);
            var outputPath = Path.Combine(Path.Combine("data", ImageFolder, $"{imageName}.{size}.jpg"));

            if (File.Exists(outputPath))
            {
                return outputPath;
            }
            
            var response = await _client.GetAsync($"http://image.tmdb.org/t/p/{size}{imageUrl}?api_key={_key}");
            using (var imageStream = await response.Content.ReadAsStreamAsync())
            using (var outputStream = File.OpenWrite(outputPath))
            {
                await imageStream.CopyToAsync(outputStream);
            }
            
            return outputPath;
        }
        
        private async Task<string> Fetch(string url)
        {
            var content = await _cachingService.GetAsync(url);

            if (string.IsNullOrEmpty(content))
            {
                var response = await _client.GetAsync(url);
                content = await response.Content.ReadAsStringAsync();
                
                await _cachingService.SetAsync(url, content, CacheKind.WebRequest);
            }

            return content;
        }

    }
}