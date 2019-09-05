using System;
using System.IO;
using System.Linq;
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
        protected IDetailsMapper Mapper;
        protected ILogger Logger;
        
        private readonly HttpClient Client = new HttpClient
        {
            DefaultRequestHeaders =
            {
                {"User-Agent", "NxPlx"}
            }
        };

        protected DetailsApiBase(ICachingService cachingService, IDetailsMapper mapper, ILogger logger)
        {
            CachingService = cachingService;
            Logger = logger;
            Mapper = mapper;
        }
        
        protected async Task<(string content, bool cached)> FetchInternal(string url)
        {
            var cached = true;
            var content = await CachingService.GetAsync(url);

            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine($"{DateTime.Now.ToLongTimeString()}\t{url}");
                var response = await Client.GetAsync(url);
                
                content = await response.Content.ReadAsStringAsync();
                cached = false;

                if (response.IsSuccessStatusCode)
                    await CachingService.SetAsync(url, content, CacheKind.WebRequest);
            }

            return (content, cached);
        }

        protected async Task DownloadImageInternal(string url, string outputPath)
        {
            if (!File.Exists(outputPath))
            {
                var response = await Client.GetAsync(url);
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
                        Logger.Trace("Failed to download image {ImagePath}. It is already being downloaded", outputPath);
                    }
                }
                
            }
        }
        
        public abstract Task<FilmResult[]> SearchMovies(string title, int year);
        public abstract Task<SeriesResult[]> SearchTvShows(string name);
        public abstract Task<FilmDetails> FetchMovieDetails(int id);
        public abstract Task<SeriesDetails> FetchTvDetails(int id);
        public abstract Task<SeasonDetails> FetchTvSeasonDetails(int id, int season);
        public abstract Task DownloadImage(string size, string imageUrl);
    }
}