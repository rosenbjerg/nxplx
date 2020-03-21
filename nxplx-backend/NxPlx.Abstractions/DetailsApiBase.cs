using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Search;
using NxPlx.Models.Details.Series;

namespace NxPlx.Abstractions
{
    public abstract class DetailsApiBase : IDetailsApi
    {
        protected readonly ICachingService CachingService;
        protected ILoggingService LoggingService;
        private string _imageFolder;
        
        protected readonly HttpClient Client = new HttpClient
        {
            DefaultRequestHeaders =
            {
                {"User-Agent", "NxPlx"}
            }
        };

        protected DetailsApiBase(string imageFolder, ICachingService cachingService, ILoggingService loggingService)
        {
            CachingService = cachingService;
            LoggingService = loggingService;
            _imageFolder = imageFolder;
        }
        
        protected async Task<string?> FetchInternal(string url)
        {
            var response = await Client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                await CachingService.SetAsync(url, content, CacheKind.WebRequest);
                return content;
            }

            return default;
        }

        protected async Task DownloadImageInternal(string url, string size, string imageName, bool first = true)
        {
            var sizeDir = Path.Combine(_imageFolder, size);
            var outputPath = Path.Combine(Path.Combine(sizeDir, $"{imageName}.jpg"));
            Directory.CreateDirectory(sizeDir);
            
            if (!File.Exists(outputPath))
            {
                try
                {
                    var response = await Client.GetAsync(url);
                    using var imageStream = await response.Content.ReadAsStreamAsync();
                    using var outputStream = File.OpenWrite(outputPath);
                    await imageStream.CopyToAsync(outputStream);
                }
                catch (HttpRequestException)
                {
                    if (first)
                        await DownloadImageInternal(url, size, imageName, false);
                    else 
                        LoggingService.Warn("Failed to download image {ImagePath} twice. Connection issues", outputPath);
                }
                catch (IOException)
                {
                    LoggingService.Trace("Failed to download image {ImagePath}. It is already being downloaded", outputPath);
                }
            }
        }
        
        public abstract Task<DetailsSearchResult[]> SearchMovies(string title, int year);
        public abstract Task<DetailsSearchResult[]> SearchTvShows(string name);
        public abstract Task<FilmDetails> FetchMovieDetails(int id, string language);
        public abstract Task<SeriesDetails> FetchTvDetails(int id, string language);
        public abstract Task<SeasonDetails> FetchTvSeasonDetails(int id, int season, string language);
        public abstract Task DownloadImage(string size, string imageUrl);
    }
}