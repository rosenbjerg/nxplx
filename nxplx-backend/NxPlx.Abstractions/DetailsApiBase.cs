using System.Net.Http;
using System.Threading.Tasks;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Search;
using NxPlx.Models.Details.Series;

namespace NxPlx.Abstractions
{
    public abstract class DetailsApiBase : IDetailsApi
    {
        private ICachingService _cachingService;
        protected IDetailsMapper Mapper;
        protected ILogger Logger;
        
        protected readonly HttpClient Client = new HttpClient
        {
            DefaultRequestHeaders =
            {
                {"User-Agent", "NxPlx"}
            }
        };

        protected DetailsApiBase(ICachingService cachingService, IDetailsMapper mapper, ILogger logger)
        {
            _cachingService = cachingService;
            Logger = logger;
            Mapper = mapper;
        }

        protected async Task<(string content, bool cached)> FetchInternal(string url)
        {
            var cached = true;
            var content = await _cachingService.GetAsync(url);

            if (string.IsNullOrEmpty(content))
            {
                var response = await Client.GetAsync(url);
                content = await response.Content.ReadAsStringAsync();
                cached = false;

                if (response.IsSuccessStatusCode)
                    await _cachingService.SetAsync(url, content, CacheKind.WebRequest);
            }

            return (content, cached);
        }
        
        public abstract Task<FilmResult[]> SearchMovies(string title, int year);
        public abstract Task<SeriesResult[]> SearchTvShows(string name);
        public abstract Task<FilmDetails> FetchMovieDetails(int id);
        public abstract Task<SeriesDetails> FetchTvDetails(int id);
        public abstract Task<SeasonDetails> FetchTvSeasonDetails(int id, int season);
        public abstract Task DownloadImage(string size, string imageUrl);
    }
}