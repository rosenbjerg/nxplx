using System.Threading.Tasks;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Series;

namespace NxPlx.Application.Core
{
    public interface IDetailsApi
    {
        Task<NxPlx.Models.Details.Search.FilmResult[]> SearchMovies(string title, int year);
        Task<NxPlx.Models.Details.Search.SeriesResult[]> SearchTvShows(string name);

        Task<Genre[]> FetchMovieGenres(string language);
        Task<Genre[]> FetchTvGenres(string language);
        Task<DbFilmDetails> FetchMovieDetails(int seriesId, string language);
        Task<DbSeriesDetails> FetchTvDetails(int seriesId, string language, int[] seasons);
        Task<SeasonDetails> FetchTvSeasonDetails(int seriesId, int seasonNo, string language);
        Task<bool> DownloadImage(int width, string imageUrl, string outputFilePath);
    }
}