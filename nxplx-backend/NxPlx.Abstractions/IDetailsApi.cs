using System.Threading.Tasks;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;

namespace NxPlx.Abstractions
{
    public interface IDetailsApi
    {
        Task<Models.Details.Search.FilmResult[]> SearchMovies(string title, int year);
        Task<Models.Details.Search.SeriesResult[]> SearchTvShows(string name);

        Task<FilmDetails> FetchMovieDetails(int id, string language);
        Task<SeriesDetails> FetchTvDetails(int id, string language);
        Task<SeasonDetails> FetchTvSeasonDetails(int id, int season, string language);
        Task DownloadImage(string size, string imageUrl);
    }
}