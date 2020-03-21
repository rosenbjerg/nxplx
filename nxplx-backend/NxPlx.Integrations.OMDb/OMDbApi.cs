using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using NxPlx.Abstractions;
using NxPlx.Configuration;
using NxPlx.Integrations.OMDb.Models;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Search;
using NxPlx.Models.Details.Series;
using TokenBucket;
using SearchResult = NxPlx.Integrations.OMDb.Models.SearchResult;

namespace NxPlx.Integrations.TMDb
{
    public class OMDbApi : DetailsApiBase
    {
        private readonly string _baseUrl;
        private readonly TMDbMapper _mapper;
        
        public OMDbApi(ICachingService cachingService, ILoggingService loggingService) 
            : base(ConfigurationService.Current.ImageFolder, cachingService,loggingService)
        {
            _mapper = new TMDbMapper();
            _baseUrl = "https://www.omdbapi.com?apikey=815a3c61";
        }


        private readonly ITokenBucket _bucket = TokenBuckets.Construct()
            .WithCapacity(10)
            .WithFixedIntervalRefillStrategy(10, TimeSpan.FromSeconds(3))
            .Build();


        private async Task<string?> Fetch(string url)
        {
            {
                var cachedContent = await CachingService.GetAsync(url);
                if (!string.IsNullOrEmpty(cachedContent)) return cachedContent;
            }
            
            _bucket.Consume(1);
            var response = await Client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                await CachingService.SetAsync(url, content, CacheKind.WebRequest);
                return content;
            }

            return null;
        }

        public override async Task<DetailsSearchResult[]> SearchMovies(string title, int year)
        {
            var encodedTitle = HttpUtility.UrlEncode(title);

            if (title == "" || encodedTitle == "")
            {
                return new FilmResult[0];
            }
            
            var encodedYear = year < 2 ? "" : $"&y={year}";
            var url = $"{_baseUrl}&s={encodedTitle}{encodedYear}&type=movie";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<SearchResult>(content);

            try
            {

                return _mapper.Map<SearchResult, DetailsSearchResult[]>(tmdbObj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public override async Task<DetailsSearchResult[]> SearchTvShows(string title)
        {
            var encodedTitle = HttpUtility.UrlEncode(title);
            var url = $"{_baseUrl}&s={encodedTitle}&type=series";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<SearchResult>(content);

            var mapped = _mapper.Map<SearchResult, DetailsSearchResult[]>(tmdbObj);
            
            return mapped;
        }

        public override async Task<FilmDetails> FetchMovieDetails(int id, string language)
        {
            var url = $"{_baseUrl}/movie/{id}?language={language}";
            
            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<MovieDetails>(content);
            
            return _mapper.Map<MovieDetails, FilmDetails>(tmdbObj);
        }
        
        public override async Task<SeriesDetails> FetchTvDetails(int id, string language)
        {
            var url = $"{_baseUrl}/tv/{id}?language={language}";
            
            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<TvDetails>(content);
            var mapped = _mapper.Map<TvDetails, SeriesDetails>(tmdbObj);
            var seasonDetailsTasks = mapped.Seasons.Select(s => FetchTvSeasonDetails(id, s.SeasonNumber, language));
            var seasonDetails = await Task.WhenAll(seasonDetailsTasks);
            mapped.Seasons = seasonDetails.ToList();
            
            return mapped;
        }
        
        public override async Task<SeasonDetails> FetchTvSeasonDetails(int id, int season, string language)
        {
            var url = $"{_baseUrl}/tv/{id}/season/{season}?language={language}";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<TvSeasonDetails>(content);
            return _mapper.Map<TvSeasonDetails, SeasonDetails>(tmdbObj);
        }
        
        public override async Task DownloadImage(string size, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            var imageName = Path.GetFileNameWithoutExtension(imageUrl.Trim('/'));
            await DownloadImageInternal($"https://image.tmdb.org/t/p/{size}{imageUrl}", size, imageName);
        }
    }
}