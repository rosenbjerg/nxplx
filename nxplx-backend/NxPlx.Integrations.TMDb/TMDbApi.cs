using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Logging;
using NxPlx.Application.Core.Settings;
using NxPlx.Integrations.TMDb.Models.Movie;
using NxPlx.Integrations.TMDb.Models.Search;
using NxPlx.Integrations.TMDb.Models.Tv;
using NxPlx.Integrations.TMDb.Models.TvSeason;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Search;
using NxPlx.Models.Details.Series;
using TokenBucket;

namespace NxPlx.Integrations.TMDb
{
    public class TMDbApi : DetailsApiBase
    {
        private const string BaseUrl = "https://api.themoviedb.org/3";
        private readonly TMDbMapper _mapper;
        
        public TMDbApi(
            FolderSettings folderSettings,
            ApiKeySettings apiKeySettings,
            IDistributedCache cachingService,
            SystemLogger systemLogger) 
            : base(folderSettings.Images, cachingService, systemLogger)
        {
            _mapper = new TMDbMapper();
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKeySettings.TMDB}");
        }

        private readonly ITokenBucket _bucket = TokenBuckets.Construct()
            .WithCapacity(10)
            .WithFixedIntervalRefillStrategy(10, TimeSpan.FromSeconds(3))
            .Build();
        private readonly ITokenBucket _imageBucket = TokenBuckets.Construct()
            .WithCapacity(100)
            .WithFixedIntervalRefillStrategy(100, TimeSpan.FromSeconds(3))
            .Build();
        
        private async Task<string> Fetch(string url)
        {
            {
                var cachedContent = await CachingService.GetStringAsync(url);
                if (!string.IsNullOrEmpty(cachedContent)) return cachedContent;
            }

            _bucket.Consume(1);
            
            var content = await FetchInternal(url);
            
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
            var url = $"{BaseUrl}/search/movie?query={encodedTitle}{encodedYear}";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<SearchResult<MovieResult>>(content);

            try
            {

                return _mapper.Map<SearchResult<MovieResult>, FilmResult[]>(tmdbObj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public override async Task<SeriesResult[]> SearchTvShows(string name)
        {
            var encodedTitle = HttpUtility.UrlEncode(name);
            var url = $"{BaseUrl}/search/tv?query={encodedTitle}";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<SearchResult<TvShowResult>>(content);

            var mapped = _mapper.Map<SearchResult<TvShowResult>, SeriesResult[]>(tmdbObj);
            
            return mapped;
        }

        public override async Task<FilmDetails> FetchMovieDetails(int id, string language)
        {
            var url = $"{BaseUrl}/movie/{id}?language={language}";
            
            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<MovieDetails>(content);
            
            return _mapper.Map<MovieDetails, FilmDetails>(tmdbObj);
        }
        
        public override async Task<SeriesDetails> FetchTvDetails(int id, string language)
        {
            var url = $"{BaseUrl}/tv/{id}?language={language}";
            
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
            var url = $"{BaseUrl}/tv/{id}/season/{season}?language={language}";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<TvSeasonDetails>(content);
            return _mapper.Map<TvSeasonDetails, SeasonDetails>(tmdbObj);
        }
        
        public override async Task DownloadImage(string size, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            
            _imageBucket.Consume(1);
            var imageName = Path.GetFileNameWithoutExtension(imageUrl.Trim('/'));
            await DownloadImageInternal($"https://image.tmdb.org/t/p/{size}{imageUrl}", size, imageName);
        }
    }
}