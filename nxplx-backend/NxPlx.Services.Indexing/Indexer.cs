using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;
using NxPlx.Services.Database;

namespace NxPlx.Services.Index
{
    public class Indexer : IIndexer
    {
        private readonly IDetailsApi _detailsApi;
        private readonly IDatabaseMapper _databaseMapper;
        private readonly ILoggingService _loggingService;
        private readonly ICachingService _cachingService;

        public Indexer(
            IDetailsApi detailsApi,
            IDatabaseMapper databaseMapper,
            ILoggingService loggingService,
            ICachingService cachingService)
        {
            _detailsApi = detailsApi;
            _databaseMapper = databaseMapper;
            _loggingService = loggingService;
            _cachingService = cachingService;
        }

        public async Task IndexLibraries(IEnumerable<Library> libraries)
        {
            foreach (var library in libraries)
            {
                switch (library.Kind)
                {
                    case LibraryKind.Film:
                        await IndexMovieLibrary(library);
                        break;
                    case LibraryKind.Series:
                        await IndexSeriesLibrary(library);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                await _cachingService.RemoveAsync("OVERVIEW:*");
            }
        }


        private async Task IndexMovieLibrary(Library library)
        {
            var startTime = DateTime.UtcNow;
            
            var fileIndexer = new FileIndexer();
            await using var ctx = new MediaContext();

            var currentFilm = ctx.FilmFiles.Select(e => e.Path).ToHashSet();
            
            await RemoveDeletedMovies(library, currentFilm, ctx, startTime);

            var newFilm = fileIndexer.IndexFilm(currentFilm, library);
            if (newFilm.Any()) _loggingService.Info("Found {NewAmount} new film files in {LibraryName} after {ScanTime} seconds", newFilm.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
            
            var details = await FindFilmDetails(newFilm, library);
            if (details.Any()) _loggingService.Info("Downloaded details for the {NewAmount} new film found in {LibraryName}, after {DownloadTime} seconds", details.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
            
            var genres = await details.Where(d => d.Genres != null).SelectMany(d => d.Genres).GetUniqueNew();
            var productionCountries = await details.Where(d => d.ProductionCountries != null).SelectMany(d => d.ProductionCountries).GetUniqueNew(pc => pc.Iso3166_1);
            var spokenLanguages = await details.Where(d => d.SpokenLanguages != null).SelectMany(d => d.SpokenLanguages).GetUniqueNew(sl => sl.Iso639_1);
            var productionCompanies = await details.Where(d => d.ProductionCompanies != null).SelectMany(d => d.ProductionCompanies).GetUniqueNew();
            var movieCollections = await details.Where(d => d.BelongsToCollection != null).Select(d => d.BelongsToCollection).GetUniqueNew();
            
            ctx.AddRange(genres);
            ctx.AddRange(productionCountries);
            ctx.AddRange(spokenLanguages);
            ctx.AddRange(productionCompanies);
            ctx.AddRange(movieCollections);
            ctx.AddRange(newFilm);
            var databaseDetails = _databaseMapper.MapMany<FilmDetails, DbFilmDetails>(details);
            var newDetails = await databaseDetails.GetUniqueNew();
            await ctx.AddRangeAsync(newDetails);
            
            await ctx.SaveChangesAsync();
            _loggingService.Info("Finished saving new film, found in {LibraryName}, to database after {SaveTime} seconds", library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

            var imageDownloads = IndexingHelperFunctions.AccumulateImageDownloads(details, productionCompanies, movieCollections);
            await IndexingHelperFunctions.DownloadImages(_detailsApi, imageDownloads);
            
            _loggingService.Info("Indexing film in {LibraryName} took {Elapsed} seconds", library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
        }

        private async Task RemoveDeletedMovies(
            Library library, HashSet<string> currentFilm, MediaContext mediaContext, DateTime startTime)
        {
            var deletedFilmPaths = currentFilm.Where(path => !File.Exists(path)).ToList();
            if (deletedFilmPaths.Any())
            {
                var deletedFilm = await mediaContext.FilmFiles.Where(ef => deletedFilmPaths.Contains(ef.Path)).ToListAsync();
                await RemovePrefsAndProgressesFromDeleted(deletedFilm.Select(df => df.Id));
                mediaContext.SubtitleFiles.RemoveRange(deletedFilm.SelectMany(e => e.Subtitles));
                mediaContext.FilmFiles.RemoveRange(deletedFilm);

                await mediaContext.SaveChangesAsync();
                _loggingService.Info(
                    "Deleted {DeletedAmount} film from {LibraryName} because files were removed, after {ScanTime} seconds",
                    deletedFilm.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
            }
        }
        private async Task RemoveDeletedEpisodes(
            Library library, HashSet<string> currentEpisodes, MediaContext mediaContext, DateTime startTime)
        {
            var deletedEpisodePaths = currentEpisodes.Where(path => !File.Exists(path)).ToList();
            if (deletedEpisodePaths.Any())
            {
                var deletedEpisodes = await mediaContext.EpisodeFiles.Where(ef => deletedEpisodePaths.Contains(ef.Path)).ToListAsync();
                await RemovePrefsAndProgressesFromDeleted(deletedEpisodes.Select(df => df.Id));
                mediaContext.SubtitleFiles.RemoveRange(deletedEpisodes.SelectMany(e => e.Subtitles));
                mediaContext.EpisodeFiles.RemoveRange(deletedEpisodes);

                await mediaContext.SaveChangesAsync();
                _loggingService.Info(
                    "Deleted {DeletedAmount} episodes from {LibraryName} because files were removed, after {ScanTime} seconds",
                    deletedEpisodes.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
            }
        }

        private static async Task RemovePrefsAndProgressesFromDeleted(IEnumerable<int> deletedIds)
        {
            await using var userContext = new UserContext();

            var subtitlePreferences = userContext.SubtitlePreferences.Where(sp => deletedIds.Contains(sp.FileId));
            userContext.SubtitlePreferences.RemoveRange(subtitlePreferences);

            var watchingProgresses = userContext.WatchingProgresses.Where(wp => deletedIds.Contains(wp.FileId));
            userContext.WatchingProgresses.RemoveRange(watchingProgresses);

            await userContext.SaveChangesAsync();
        }


        private async Task<List<FilmDetails>> FindFilmDetails(List<FilmFile> newFilm, Library library)
        {
            var allDetails = new HashSet<FilmDetails>();
            var functionCache = new FunctionCache<int, string, FilmDetails>(FetchFilmDetails);
            
            foreach (var filmFile in newFilm)
            {
                var searchResults = await _detailsApi.SearchMovies(filmFile.Title, filmFile.Year);
                if (searchResults == null || !searchResults.Any())
                    searchResults = await _detailsApi.SearchMovies(filmFile.Title, 0);

                if (searchResults == null || !searchResults.Any())
                {
                    filmFile.FilmDetailsId = null;
                    continue;
                }

                var filmDetails = await functionCache.Invoke(searchResults[0].Id, library.Language);
                filmFile.FilmDetailsId = filmDetails.Id;
                allDetails.Add(filmDetails);
            }

            return allDetails.ToList();
        }
        private Task<FilmDetails> FetchFilmDetails(int id, string language) => _detailsApi.FetchMovieDetails(id, language);


        private async Task IndexSeriesLibrary(Library library)
        {
            var startTime = DateTime.UtcNow;
            
            var fileIndexer = new FileIndexer();
            await using var ctx = new MediaContext();
            await using var transaction = ctx.Database.BeginTransaction();
            
            var currentEpisodes = ctx.EpisodeFiles.Select(e => e.Path).ToHashSet();
            
            await RemoveDeletedEpisodes(library, currentEpisodes, ctx, startTime);
            
            var newEpisodes = fileIndexer.IndexEpisodes(currentEpisodes, library);
            if (newEpisodes.Any()) _loggingService.Info("Found {NewAmount} new episode files in {LibraryName} after {ScanTime} seconds", newEpisodes.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

            var details = await FindSeriesDetails(newEpisodes, library);
            if (details.Any()) _loggingService.Info("Downloaded details for the {NewAmount} new series found in {LibraryName}, after {DownloadTime} seconds", details.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

            var genres = await details.Where(d => d.Genres != null).SelectMany(d => d.Genres).GetUniqueNew();
            var networks = await details.Where(d => d.Networks != null).SelectMany(d => d.Networks).GetUniqueNew();
            var creators = await details.Where(d => d.CreatedBy != null).SelectMany(d => d.CreatedBy).GetUniqueNew();
            var productionCompanies = await details.Where(d => d.ProductionCompanies != null).SelectMany(d => d.ProductionCompanies).GetUniqueNew();
            
            ctx.AddRange(newEpisodes);
            ctx.AddRange(genres);
            ctx.AddRange(networks);
            ctx.AddRange(creators);
            ctx.AddRange(productionCompanies);
            var databaseDetails = _databaseMapper.MapMany<SeriesDetails, DbSeriesDetails>(details).ToList();
            await ctx.AddOrUpdate(databaseDetails);
            
            await ctx.SaveChangesAsync();
            _loggingService.Info("Finished saving new series, found in {LibraryName}, to database after {SaveTime} seconds", library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

            var imageDownloads = IndexingHelperFunctions.AccumulateImageDownloads(details, networks, productionCompanies);
            await IndexingHelperFunctions.DownloadImages(_detailsApi, imageDownloads);
            
            _loggingService.Info("Indexing episodes in {LibraryName} took {Elapsed} seconds", library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
        }


        private async Task<List<SeriesDetails>> FindSeriesDetails(List<EpisodeFile> newEpisodes, Library library)
        {
            var allDetails = new HashSet<SeriesDetails>();
            var functionCache = new FunctionCache<int, string, SeriesDetails>(FetchSeriesDetails);

            foreach (var episodeFile in newEpisodes)
            {
                var searchResults = await _detailsApi.SearchTvShows(episodeFile.Name);

                var details = searchResults != null && searchResults.Any() ? searchResults[0] : null;

                if (details == null) 
                    continue;

                var seriesDetails = await functionCache.Invoke(details.Id, library.Language);
                episodeFile.SeriesDetailsId = seriesDetails.Id;
                allDetails.Add(seriesDetails);
            }

            return allDetails.ToList();
        }
        private Task<SeriesDetails> FetchSeriesDetails(int id, string language) => _detailsApi.FetchTvDetails(id, language);

    }
}