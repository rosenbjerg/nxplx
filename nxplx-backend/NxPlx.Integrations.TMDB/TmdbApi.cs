using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.IoC;
using NxPlx.Integrations.TMDBApi.Models.Movie;
using NxPlx.Integrations.TMDBApi.Models.Search;
using NxPlx.Integrations.TMDBApi.Models.Tv;
using NxPlx.Integrations.TMDBApi.Models.TvSeason;
using NxPlx.Services.Caching;

namespace NxPlx.Integrations.TMDBApi
{

    public interface IDetailsApi
    {
        
    }
    
    public class TmdbApi
    {
        private const string ImageFolder = "images";

        private const string BaseUrl = "https://api.themoviedb.org/3";
        
        private string _key;
        private ICachingService _cachingService;
        private HttpClient _client = new HttpClient
        {
            DefaultRequestHeaders =
            {
                {"User-Agent", "NxPlx"}
            }
        };


        public TmdbApi(string apiKey)
        {
            _key = apiKey;
            
            var container = new ResolveContainer();
            _cachingService = container.Resolve<ICachingService>();
            Directory.CreateDirectory(Path.Combine("data", ImageFolder));
        }
        

        public async Task<SearchResult<MovieResult>> SearchMovies(string title, int year)
        {
            var encodedTitle = HttpUtility.UrlEncode(title);
            var encodedYear = year == 0 ? "" : $"&year={year}";
            var url = $"{BaseUrl}/search/movie?api_key={_key}&query={encodedTitle}{encodedYear}";

            var content = await Fetch(url);
            return JsonConvert.DeserializeObject<SearchResult<MovieResult>>(content);
        }
        
        public async Task<SearchResult<TvShowResult>> SearchTvShows(string name)
        {
            var encodedTitle = HttpUtility.UrlEncode(name);
            var url = $"{BaseUrl}/search/tv?api_key={_key}&query={encodedTitle}";

            var content = await Fetch(url);
            return JsonConvert.DeserializeObject<SearchResult<TvShowResult>>(content);
        }

        public async Task<MovieDetails> FetchMovieDetails(int id)
        {
            var url = $"{BaseUrl}/movie/{id}?api_key={_key}";
            
            var content = await Fetch(url);
            return JsonConvert.DeserializeObject<MovieDetails>(content);
        }
        
        public async Task<TvDetails> FetchTvDetails(int id)
        {
            var url = $"{BaseUrl}/tv/{id}?api_key={_key}";
            
            var content = await Fetch(url);
            return JsonConvert.DeserializeObject<TvDetails>(content);
        }
        
        public async Task<TvSeasonDetails> FetchTvSeasonDetails(int id, int season)
        {
            var url = $"{BaseUrl}/tv/{id}/season/{season}?api_key={_key}";

            var content = await Fetch(url);
            return JsonConvert.DeserializeObject<TvSeasonDetails>(content);
        }
        
        public async Task DownloadImage(string size, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            
            var imageName = Path.GetFileNameWithoutExtension(imageUrl.Trim('/'));
            var outputPath = Path.Combine(Path.Combine("data", ImageFolder, $"{imageName}-{size}.jpg"));

            if (!File.Exists(outputPath))
            {
                var response = await _client.GetAsync($"https://image.tmdb.org/t/p/{size}{imageUrl}?api_key={_key}");
                using (var imageStream = await response.Content.ReadAsStreamAsync())
                {
                    try
                    {
                        using (var outputStream = File.OpenWrite(outputPath))
                        {
                         await imageStream.CopyToAsync(outputStream);
                        }
                    }
                    catch (IOException e)
                    {
                        
                    }
                }
                
            }
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