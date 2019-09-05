using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Esendex.TokenBucket;
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

namespace NxPlx.Integrations.TMDBApi
{
    public class TmdbApi : DetailsApiBase
    {
        private const int ThrottlingMs = 10000 / 40;
        private const string BaseUrl = "https://api.themoviedb.org/3";
        
        private string _key;
        private string _imageFolder;

        public TmdbApi(ICachingService cachingService, IDetailsMapper mapper, ILogger logger) : base(cachingService, mapper, logger)
        {
            var cfg = ConfigurationService.Current;
            _key = cfg.TMDbApiKey;
            _imageFolder = cfg.ImageFolder;
            Directory.CreateDirectory(_imageFolder);
        }

        private readonly ITokenBucket _bucket = TokenBuckets.Construct()
            .WithCapacity(10)
            .WithFixedIntervalRefillStrategy(10, TimeSpan.FromSeconds(4))
            .Build();
        private async Task<string> Fetch(string url)
        {
            {
                var cachedContent = await CachingService.GetAsync(url);
                if (!string.IsNullOrEmpty(cachedContent)) return cachedContent;
            }

            _bucket.Consume(1);
            
            var (content, cached) = await FetchInternal(url);
            
            if (content.StartsWith("{\"status_code\":25"))
            {
                return await Fetch(url);
            }

            return content;
        }

        public override async Task<FilmResult[]> SearchMovies(string title, int year)
        {
            var encodedTitle = HttpUtility.UrlEncode(title);
            var encodedYear = year == 0 ? "" : $"&year={year}";
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

        public override async Task<FilmDetails> FetchMovieDetails(int id)
        {
            var url = $"{BaseUrl}/movie/{id}?api_key={_key}";
            
            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<MovieDetails>(content);
            
            return Mapper.Map<MovieDetails, FilmDetails>(tmdbObj);
        }
        
        public override async Task<SeriesDetails> FetchTvDetails(int id)
        {
            var url = $"{BaseUrl}/tv/{id}?api_key={_key}";
            
            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<TvDetails>(content);
            var mapped = Mapper.Map<TvDetails, SeriesDetails>(tmdbObj);
            var seasonDetailsTasks = mapped.Seasons.Select(s => FetchTvSeasonDetails(id, s.SeasonNumber));
            var seasonDetails = await Task.WhenAll(seasonDetailsTasks);
            mapped.Seasons = seasonDetails.ToList();
            
            return mapped;
        }
        
        public override async Task<SeasonDetails> FetchTvSeasonDetails(int id, int season)
        {
            var url = $"{BaseUrl}/tv/{id}/season/{season}?api_key={_key}";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<TvSeasonDetails>(content);
            return Mapper.Map<TvSeasonDetails, SeasonDetails>(tmdbObj);
        }
        
        public override async Task DownloadImage(string size, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            
            var imageName = Path.GetFileNameWithoutExtension(imageUrl.Trim('/'));
            var outputPath = Path.Combine(Path.Combine(_imageFolder, $"{imageName}-{size}.jpg"));
            await DownloadImageInternal($"https://image.tmdb.org/t/p/{size}{imageUrl}", outputPath);
        }
    }
}