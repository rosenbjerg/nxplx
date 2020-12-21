using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Search;
using NxPlx.Models.Details.Series;

namespace NxPlx.Application.Core
{
    public abstract class DetailsApiBase : IDetailsApi
    {
        protected readonly IDistributedCache CachingService;
        protected readonly ILogger<IDetailsApi> Logger;
        private readonly string _imageFolder;
        
        protected static readonly HttpClient Client = new HttpClient
        {
            DefaultRequestHeaders =
            {
                {"User-Agent", "NxPlx"}
            }
        };

        protected DetailsApiBase(string imageFolder, IDistributedCache cachingService, ILogger<IDetailsApi> logger)
        {
            CachingService = cachingService;
            Logger = logger;
            _imageFolder = imageFolder;
        }
        
        protected async Task<string?> FetchInternal(string cacheKey, string url)
        {
            var response = await Client.GetAsync(url);

            if (!response.IsSuccessStatusCode) return default;
            
            var content = await response.Content.ReadAsStringAsync();
            await CachingService.SetStringAsync(cacheKey, content, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(6)
            });
            return content;
        }

        protected async Task<bool> DownloadImageInternal(string imageUrl, string outputPath, bool first = true)
        {
            if (!File.Exists(outputPath))
            {
                try
                {
                    using var response = await Client.GetAsync(imageUrl);
                    if (!response.IsSuccessStatusCode) return false;
                    await using var imageStream = await response.Content.ReadAsStreamAsync();
                    await using var outputStream = File.OpenWrite(outputPath);
                    await imageStream.CopyToAsync(outputStream);
                    return outputStream.Length > 50;
                }
                catch (HttpRequestException)
                {
                    if (first)
                        return await DownloadImageInternal(imageUrl, outputPath, false);
                    
                    Logger.LogWarning("Failed to download image {ImagePath} twice. Connection issues", outputPath);
                }
                catch (IOException)
                {
                    Logger.LogTrace("Failed to download image {ImagePath}. It is already being downloaded", outputPath);
                }
                return false;
            }
            return false;
        }
        
        public abstract Task<FilmResult[]> SearchMovies(string title, int year);
        public abstract Task<SeriesResult[]> SearchTvShows(string name);
        public abstract Task<DbFilmDetails> FetchMovieDetails(int seriesId, string language);
        public abstract Task<DbSeriesDetails> FetchTvDetails(int seriesId, string language, int[] seasons);
        public abstract Task<SeasonDetails> FetchTvSeasonDetails(int seriesId, int seasonNo, string language);
        public abstract Task<bool> DownloadImage(int width, string imageUrl, string outputFilePath);
    }
}