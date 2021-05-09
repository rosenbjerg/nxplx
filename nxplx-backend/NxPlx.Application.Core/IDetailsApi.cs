using System.Threading.Tasks;
using NxPlx.Domain.Models.Database;
using NxPlx.Domain.Models.Details;
using NxPlx.Domain.Models.Details.Series;
using NxPlx.Domain.Models.Details.Search;

namespace NxPlx.Application.Core
{
    public interface IDetailsApi
    {
        Task<FilmResult[]> SearchMovies(string title, int year);
        Task<SeriesResult[]> SearchTvShows(string name);

        Task<Genre[]> FetchMovieGenres(string language);
        Task<Genre[]> FetchTvGenres(string language);
        Task<DbFilmDetails> FetchMovieDetails(int seriesId, string language);
        Task<DbSeriesDetails> FetchTvDetails(int seriesId, string language, int[] seasons);
        Task<SeasonDetails> FetchTvSeasonDetails(int seriesId, int seasonNo, string language);
        Task<bool> DownloadImage(int width, string imageUrl, string outputFilePath);
    }
}