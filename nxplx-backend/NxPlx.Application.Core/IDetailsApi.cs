using System.Threading.Tasks;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;

namespace NxPlx.Application.Core
{
    public interface IDetailsApi
    {
        Task<NxPlx.Models.Details.Search.FilmResult[]> SearchMovies(string title, int year);
        Task<NxPlx.Models.Details.Search.SeriesResult[]> SearchTvShows(string name);

        Task<FilmDetails> FetchMovieDetails(int seriesId, string language);
        Task<SeriesDetails> FetchTvDetails(int seriesId, string language, int[] seasons);
        Task<SeasonDetails> FetchTvSeasonDetails(int seriesId, int seasonNo, string language);
        Task<bool> DownloadImage(int width, string imageUrl, string outputFilePath);
    }
}