using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using NxPlx.Abstractions;
using NxPlx.Configuration;
using NxPlx.Integrations.TMDBApi.Models.Movie;
using NxPlx.Integrations.TMDBApi.Models.Search;
using NxPlx.Integrations.TMDBApi.Models.Tv;
using NxPlx.Integrations.TMDBApi.Models.TvSeason;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Search;
using NxPlx.Models.Details.Series;
using TokenBucket;

namespace NxPlx.Integrations.TMDBApi
{
    public class TmdbApi : DetailsApiBase
    {
        private const int ThrottlingMs = 10000 / 40;
        private const string BaseUrl = "https://api.themoviedb.org/3";
        
        private string _key;

        public TmdbApi(ICachingService cachingService, IDetailsMapper mapper, ILoggingService loggingService) 
            : base(ConfigurationService.Current.ImageFolder, cachingService, mapper, loggingService)
        {
            _key = ConfigurationService.Current.TMDbApiKey;
        }

        private readonly ITokenBucket _bucket = TokenBuckets.Construct()
            .WithCapacity(10)
            .WithFixedIntervalRefillStrategy(10, TimeSpan.FromSeconds(3))
            .Build();
        
        private async Task<string> Fetch(string url)
        {
            {
                var cachedContent = await CachingService.GetAsync(url);
                if (!string.IsNullOrEmpty(cachedContent)) return cachedContent;
            }

            _bucket.Consume(1);
            
            var (content, cached) = await FetchInternal(url);
            
            if (string.IsNullOrEmpty(content) || content.StartsWith("{\"status_code\":25"))
            {
                return await Fetch(url);
            }

            return content;
        }

        public override async Task<FilmResult[]> SearchMovies(string title, int year)
        {
            var encodedTitle = HttpUtility.UrlEncode(title);

            if (title == "" || encodedTitle == "")
            {
                return new FilmResult[0];
            }
            
            var encodedYear = year < 2 ? "" : $"&year={year}";
            var url = $"{BaseUrl}/search/movie?api_key={_key}&query={encodedTitle}{encodedYear}";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<SearchResult<MovieResult>>(content);
            
            return Mapper.Map<SearchResult<MovieResult>, FilmResult[]>(tmdbObj);
        }
        
        public override async Task<SeriesResult[]> SearchTvShows(string name)
        {
            var encodedTitle = HttpUtility.UrlEncode(name);
            var url = $"{BaseUrl}/search/tv?api_key={_key}&query={encodedTitle}";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<SearchResult<TvShowResult>>(content);

            var mapped = Mapper.Map<SearchResult<TvShowResult>, SeriesResult[]>(tmdbObj);
            
            return mapped;
        }

        public override async Task<FilmDetails> FetchMovieDetails(int id, string language)
        {
            var url = $"{BaseUrl}/movie/{id}?language={language}&api_key={_key}";
            
            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<MovieDetails>(content);
            
            return Mapper.Map<MovieDetails, FilmDetails>(tmdbObj);
        }
        
        public override async Task<SeriesDetails> FetchTvDetails(int id, string language)
        {
            var url = $"{BaseUrl}/tv/{id}?language={language}&api_key={_key}";
            
            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<TvDetails>(content);
            var mapped = Mapper.Map<TvDetails, SeriesDetails>(tmdbObj);
            var seasonDetailsTasks = mapped.Seasons.Select(s => FetchTvSeasonDetails(id, s.SeasonNumber, language));
            var seasonDetails = await Task.WhenAll(seasonDetailsTasks);
            mapped.Seasons = seasonDetails.ToList();
            
            return mapped;
        }
        
        public override async Task<SeasonDetails> FetchTvSeasonDetails(int id, int season, string language)
        {
            var url = $"{BaseUrl}/tv/{id}/season/{season}?language={language}&api_key={_key}";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<TvSeasonDetails>(content);
            return Mapper.Map<TvSeasonDetails, SeasonDetails>(tmdbObj);
        }
        
        public override async Task DownloadImage(string size, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            
            var imageName = Path.GetFileNameWithoutExtension(imageUrl.Trim('/'));
            await DownloadImageInternal($"https://image.tmdb.org/t/p/{size}{imageUrl}", size, imageName);
        }
    }
}