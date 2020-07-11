using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NxPlx.Models.Details.Film;
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

        protected async Task DownloadImageInternal(string url, string size, string imageName, bool first = true)
        {
            var sizeDir = Path.Combine(_imageFolder, size);
            var outputPath = Path.Combine(Path.Combine(sizeDir, $"{imageName}.jpg"));
            Directory.CreateDirectory(sizeDir);
            
            if (!System.IO.File.Exists(outputPath))
            {
                try
                {
                    var response = await Client.GetAsync(url);
                    using var imageStream = await response.Content.ReadAsStreamAsync();
                    using var outputStream = System.IO.File.OpenWrite(outputPath);
                    await imageStream.CopyToAsync(outputStream);
                }
                catch (HttpRequestException)
                {
                    if (first)
                        await DownloadImageInternal(url, size, imageName, false);
                    else 
                        Logger.LogWarning("Failed to download image {ImagePath} twice. Connection issues", outputPath);
                }
                catch (IOException)
                {
                    Logger.LogTrace("Failed to download image {ImagePath}. It is already being downloaded", outputPath);
                }
            }
        }
        
        public abstract Task<FilmResult[]> SearchMovies(string title, int year);
        public abstract Task<SeriesResult[]> SearchTvShows(string name);
        public abstract Task<FilmDetails> FetchMovieDetails(int id, string language);
        public abstract Task<SeriesDetails> FetchTvDetails(int id, string language);
        public abstract Task<SeasonDetails> FetchTvSeasonDetails(int id, int season, string language);
        public abstract Task DownloadImage(string size, string imageUrl);
    }
}