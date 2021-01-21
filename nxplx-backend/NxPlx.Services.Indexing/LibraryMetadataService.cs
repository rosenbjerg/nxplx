using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Application.Core;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.File;

namespace NxPlx.Services.Index
{
    public class LibraryMetadataService
    {
        private readonly IDetailsApi _detailsApi;

        public LibraryMetadataService(IDetailsApi detailsApi)
        {
            _detailsApi = detailsApi;
        }

        public Task<Genre[]> FetchFilmGenres(string language) => _detailsApi.FetchMovieGenres(language);
        public Task<Genre[]> FetchSeriesGenres(string language) => _detailsApi.FetchTvGenres(language);
        public async Task<DbFilmDetails[]> FindFilmDetails(IEnumerable<FilmFile> newFilm, Library library)
        {
            var allDetails = new Dictionary<int, DbFilmDetails>();
            var functionCache = new FunctionCache<int, string, DbFilmDetails>(FetchFilmDetails);
            
            foreach (var filmFile in newFilm)
            {
                var searchResults = await _detailsApi.SearchMovies(filmFile.Title, filmFile.Year);
                if (!searchResults.Any())
                    searchResults = await _detailsApi.SearchMovies(filmFile.Title, 0);

                if (!searchResults.Any())
                {
                    filmFile.FilmDetailsId = null;
                    continue;
                }

                var actual = new Fastenshtein.Levenshtein(filmFile.Title);
                var selectedResult = searchResults.OrderBy(sr => actual.DistanceFrom(sr.Title)).First();

                var details = await functionCache.Invoke(selectedResult.Id, library.Language);
                if (details == null) continue;

                filmFile.FilmDetailsId = details.Id;
                allDetails[details.Id] = details;
            }

            return allDetails.Values.ToArray();
        }
        public async Task<DbSeriesDetails[]> FindSeriesDetails(List<EpisodeFile> newEpisodes, Library library)
        {
            await EnrichWithSeriesDetailsId(newEpisodes);
            var details = new List<DbSeriesDetails>();
            var newSeries = newEpisodes.ToLookup(e => e.SeriesDetailsId);
            
            foreach (var series in newSeries)
            {
                if (series.Key == null) continue;
                var seasons = series.Select(e => e.SeasonNumber).Distinct().ToArray();
                var seriesDetails = await _detailsApi.FetchTvDetails(series.Key.Value, library.Language, seasons);
                details.Add(seriesDetails);
            }

            return details.ToArray();
        }
        private Task<DbFilmDetails> FetchFilmDetails(int id, string language) => _detailsApi.FetchMovieDetails(id, language);
        private async Task EnrichWithSeriesDetailsId(List<EpisodeFile> newEpisodes)
        {
            var functionCache = new FunctionCache<string, Models.Details.Search.SeriesResult[]>(_detailsApi.SearchTvShows);
            
            foreach (var episodeFile in newEpisodes)
            {
                var searchResults = await functionCache.Invoke(episodeFile.Name);
                if (searchResults == null || !searchResults.Any())
                {
                    episodeFile.SeriesDetailsId = null;
                    continue;
                }
                
                var actual = new Fastenshtein.Levenshtein(episodeFile.Name);
                var selectedResult = searchResults.OrderBy(sr => actual.DistanceFrom(sr.Name)).First();
                
                episodeFile.SeriesDetailsId = selectedResult.Id;
            }
        }
    }
}