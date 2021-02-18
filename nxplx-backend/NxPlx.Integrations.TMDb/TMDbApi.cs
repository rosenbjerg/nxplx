using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using NxPlx.Integrations.TMDb.Models;
using NxPlx.Integrations.TMDb.Models.Movie;
using NxPlx.Integrations.TMDb.Models.Search;
using NxPlx.Integrations.TMDb.Models.Tv;
using NxPlx.Integrations.TMDb.Models.TvSeason;
using NxPlx.Domain.Models.Database;
using NxPlx.Domain.Models.Details.Search;
using NxPlx.Domain.Models.Details.Series;
using TokenBucket;

namespace NxPlx.Integrations.TMDb
{
    public class TMDbApi : DetailsApiBase
    {
        private const string BaseUrl = "https://api.themoviedb.org/3";
        private const string CachePrefix = "TmdbCache";
        private readonly IMapper _mapper;
        
        public TMDbApi(
            ApiKeyOptions apiKeySettings,
            IDistributedCache cachingService,
            IMapper mapper,
            ILogger<TMDbApi> logger) 
            : base(cachingService, logger)
        {
            _mapper = mapper;
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKeySettings.TmdbKey}");
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
            var cacheKey = $"{CachePrefix}:{url}";
            {
                var cachedContent = await CachingService.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedContent)) return cachedContent;
            }

            _bucket.Consume(1);
            
            var content = await FetchInternal(cacheKey, url);
            
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
                return new FilmResult[0];
            
            var encodedYear = year < 2 ? "" : $"&year={year}";
            var url = $"{BaseUrl}/search/movie?query={encodedTitle}{encodedYear}";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<SearchResult<MovieResult>>(content);
            if (tmdbObj == null) return Array.Empty<FilmResult>();
            return tmdbObj.results.Take(10).Select(res => _mapper.Map<MovieResult, FilmResult>(res)).ToArray();
        }
        
        public override async Task<SeriesResult[]> SearchTvShows(string name)
        {
            var encodedTitle = HttpUtility.UrlEncode(name);
            var url = $"{BaseUrl}/search/tv?query={encodedTitle}";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<SearchResult<TvShowResult>>(content);
            if (tmdbObj == null) return Array.Empty<SeriesResult>();
            return tmdbObj.results.Take(10).Select(res => _mapper.Map<TvShowResult, SeriesResult>(res)).ToArray();
        }

        public override async Task<Domain.Models.Details.Genre[]> FetchMovieGenres(string language)
        {
            var url = $"{BaseUrl}/genre/movie/list?language={language}";
            var content = await Fetch(url);
            var genreList = JsonConvert.DeserializeObject<GenreList>(content);
            return genreList.Genres.Select(g => _mapper.Map<Genre, Domain.Models.Details.Genre>(g)).ToArray();
        }

        public override async Task<Domain.Models.Details.Genre[]> FetchTvGenres(string language)
        {
            var url = $"{BaseUrl}/genre/tv/list?language={language}";
            var content = await Fetch(url);
            var genreList = JsonConvert.DeserializeObject<GenreList>(content);
            return genreList.Genres.Select(g => _mapper.Map<Genre, Domain.Models.Details.Genre>(g)).ToArray();
        }

        public override async Task<DbFilmDetails> FetchMovieDetails(int seriesId, string language)
        {
            var url = $"{BaseUrl}/movie/{seriesId}?language={language}";
            
            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<MovieDetails>(content);
            
            return _mapper.Map<MovieDetails, DbFilmDetails>(tmdbObj);
        }
        
        public override async Task<DbSeriesDetails> FetchTvDetails(int seriesId, string language, int[] seasons)
        {
            var url = $"{BaseUrl}/tv/{seriesId}?language={language}";
            
            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<TvDetails>(content);
            var mapped = _mapper.Map<TvDetails, DbSeriesDetails>(tmdbObj);
            var seasonDetailsTasks = mapped!.Seasons.Where(s => seasons.Contains(s.SeasonNumber)).Select(s => FetchTvSeasonDetails(seriesId, s.SeasonNumber, language));
            var seasonDetails = await Task.WhenAll(seasonDetailsTasks);
            mapped.Seasons = seasonDetails.ToList();
            
            return mapped;
        }

        public override async Task<SeasonDetails> FetchTvSeasonDetails(int seriesId, int seasonNo, string language)
        {
            var url = $"{BaseUrl}/tv/{seriesId}/season/{seasonNo}?language={language}";

            var content = await Fetch(url);
            var tmdbObj = JsonConvert.DeserializeObject<TvSeasonDetails>(content);
            return _mapper.Map<TvSeasonDetails, SeasonDetails>(tmdbObj);
        }
        
        public override async Task<bool> DownloadImage(int width, string imageUrl, string outputFilePath)
        {
            if (string.IsNullOrEmpty(imageUrl)) return false;
            
            _imageBucket.Consume(1);
            return await DownloadImageInternal($"https://image.tmdb.org/t/p/w{width}/{imageUrl}", outputFilePath);
        }
    }
}