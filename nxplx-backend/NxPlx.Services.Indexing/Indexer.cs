using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Logging;
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
        private readonly ILoggingService _systemLogger;
        private readonly IDistributedCache _cachingService;
        private readonly DatabaseContext _context;

        public Indexer(
            IDetailsApi detailsApi,
            IDatabaseMapper databaseMapper,
            SystemLogger systemLogger,
            IDistributedCache cachingService,
            DatabaseContext context)
        {
            _detailsApi = detailsApi;
            _databaseMapper = databaseMapper;
            _systemLogger = systemLogger;
            _cachingService = cachingService;
            _context = context;
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
            _systemLogger.Info("Started indexing {LibraryName}", library.Name);
            
            var fileIndexer = new FileIndexer();

            var currentFilm = new HashSet<string>(await _context.FilmFiles.Select(e => e.Path).ToListAsync());
            
            await RemoveDeletedMovies(library, currentFilm, _context, startTime);

            var newFilm = fileIndexer.IndexFilm(currentFilm, library);
            if (newFilm.Any()) _systemLogger.Info("Found {NewAmount} new film files in {LibraryName} after {ScanTime} seconds", newFilm.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
            
            var details = await FindFilmDetails(newFilm, library);
            if (details.Any()) _systemLogger.Info("Downloaded details for the {NewAmount} new film found in {LibraryName}, after {DownloadTime} seconds", details.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
            
            var genres = await details.Where(d => d.Genres != null).SelectMany(d => d.Genres).GetUniqueNew();
            var productionCountries = await details.Where(d => d.ProductionCountries != null).SelectMany(d => d.ProductionCountries).GetUniqueNew(pc => pc.Iso3166_1);
            var spokenLanguages = await details.Where(d => d.SpokenLanguages != null).SelectMany(d => d.SpokenLanguages).GetUniqueNew(sl => sl.Iso639_1);
            var productionCompanies = await details.Where(d => d.ProductionCompanies != null).SelectMany(d => d.ProductionCompanies).GetUniqueNew();
            var movieCollections = await details.Where(d => d.BelongsToCollection != null).Select(d => d.BelongsToCollection).GetUniqueNew();
            
            _context.AddRange(genres);
            _context.AddRange(productionCountries);
            _context.AddRange(spokenLanguages);
            _context.AddRange(productionCompanies);
            _context.AddRange(movieCollections);
            _context.FilmFiles.AddRange(newFilm);
            var databaseDetails = _databaseMapper.Map<FilmDetails, DbFilmDetails>(details).ToList();
            databaseDetails.ForEach(film => film.Added = DateTime.UtcNow);
            var newDetails = await databaseDetails.GetUniqueNew();
            await _context.AddRangeAsync(newDetails);
            
            await _context.SaveChangesAsync();
            _systemLogger.Info("Finished saving new film, found in {LibraryName}, to database after {SaveTime} seconds", library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

            var imageDownloads = IndexingHelperFunctions.AccumulateImageDownloads(details, productionCompanies, movieCollections);
            await IndexingHelperFunctions.DownloadImages(_detailsApi, imageDownloads);
            
            _systemLogger.Info("Indexing film in {LibraryName} took {Elapsed} seconds", library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
        }

        private async Task RemoveDeletedMovies(
            Library library, HashSet<string> currentFilm, DatabaseContext databaseContext, DateTime startTime)
        {
            var deletedFilmPaths = currentFilm.Where(path => !File.Exists(path)).ToList();
            if (deletedFilmPaths.Any())
            {
                var deletedFilm = await databaseContext.FilmFiles.Where(ef => deletedFilmPaths.Contains(ef.Path)).ToListAsync();
                await RemoveWatchingProgress(databaseContext, deletedFilm.Select(e => e.Id).ToList());
                databaseContext.FilmFiles.RemoveRange(deletedFilm);

                await databaseContext.SaveChangesAsync();
                _systemLogger.Info(
                    "Deleted {DeletedAmount} film from {LibraryName} because files were removed, after {ScanTime} seconds",
                    deletedFilm.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
            }
        }
        private async Task RemoveDeletedEpisodes(
            Library library, HashSet<string> currentEpisodes, DatabaseContext databaseContext, DateTime startTime)
        {
            var deletedEpisodePaths = currentEpisodes.Where(path => !File.Exists(path)).ToList();
            if (deletedEpisodePaths.Any())
            {
                var deletedEpisodes = await databaseContext.EpisodeFiles.Where(ef => deletedEpisodePaths.Contains(ef.Path)).ToListAsync();
                await RemoveWatchingProgress(databaseContext, deletedEpisodes.Select(e => e.Id).ToList());
                databaseContext.EpisodeFiles.RemoveRange(deletedEpisodes);

                await databaseContext.SaveChangesAsync();
                _systemLogger.Info(
                    "Deleted {DeletedAmount} episodes from {LibraryName} because files were removed, after {ScanTime} seconds",
                    deletedEpisodes.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
            }
        }

        private async Task RemoveWatchingProgress(DatabaseContext databaseContext, List<int> fileIds)
        {
            var progress = await databaseContext.WatchingProgresses.Where(wp => fileIds.Contains(wp.FileId)).ToListAsync();
            databaseContext.WatchingProgresses.RemoveRange(progress);
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

                var actual = new Fastenshtein.Levenshtein(filmFile.Title);
                var selectedResult = searchResults.OrderBy(sr => actual.DistanceFrom(sr.Title)).First();

                var filmDetails = await functionCache.Invoke(selectedResult.Id, library.Language);
                filmFile.FilmDetailsId = filmDetails.Id;
                allDetails.Add(filmDetails);
            }

            return allDetails.ToList();
        }
        private Task<FilmDetails> FetchFilmDetails(int id, string language) => _detailsApi.FetchMovieDetails(id, language);


        private async Task IndexSeriesLibrary(Library library)
        {
            var startTime = DateTime.UtcNow;
            _systemLogger.Info("Started indexing {LibraryName}", library.Name);
            
            var fileIndexer = new FileIndexer();
            
            var currentEpisodes = new HashSet<string>(await _context.EpisodeFiles.Select(e => e.Path).ToListAsync());
            
            await RemoveDeletedEpisodes(library, currentEpisodes, _context, startTime);
            
            var newEpisodes = fileIndexer.IndexEpisodes(currentEpisodes, library);
            if (newEpisodes.Any()) _systemLogger.Info("Found {NewAmount} new episode files in {LibraryName} after {ScanTime} seconds", newEpisodes.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

            var details = await FindSeriesDetails(newEpisodes, library);
            if (details.Any()) _systemLogger.Info("Downloaded details for the {NewAmount} new series found in {LibraryName}, after {DownloadTime} seconds", details.Count, library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

            var genres = await details.Where(d => d.Genres != null).SelectMany(d => d.Genres).GetUniqueNew();
            var networks = await details.Where(d => d.Networks != null).SelectMany(d => d.Networks).GetUniqueNew();
            var creators = await details.Where(d => d.CreatedBy != null).SelectMany(d => d.CreatedBy).GetUniqueNew();
            var productionCompanies = await details.Where(d => d.ProductionCompanies != null).SelectMany(d => d.ProductionCompanies).GetUniqueNew();
            
            _context.AddRange(genres);
            _context.AddRange(networks);
            _context.AddRange(creators);
            _context.AddRange(productionCompanies);
            _context.EpisodeFiles.AddRange(newEpisodes);
            var databaseDetails = _databaseMapper.Map<SeriesDetails, DbSeriesDetails>(details).ToList();
            databaseDetails.ForEach(series => series.Added = DateTime.UtcNow);
            await _context.AddOrUpdate(databaseDetails);
            
            await _context.SaveChangesAsync();
            _systemLogger.Info("Finished saving new series, found in {LibraryName}, to database after {SaveTime} seconds", library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

            var imageDownloads = IndexingHelperFunctions.AccumulateImageDownloads(details, networks, productionCompanies);
            await IndexingHelperFunctions.DownloadImages(_detailsApi, imageDownloads);
            
            _systemLogger.Info("Indexing episodes in {LibraryName} took {Elapsed} seconds", library.Name, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
        }


        private async Task<List<SeriesDetails>> FindSeriesDetails(List<EpisodeFile> newEpisodes, Library library)
        {
            var allDetails = new HashSet<SeriesDetails>();
            var functionCache = new FunctionCache<int, string, SeriesDetails>(FetchSeriesDetails);

            foreach (var episodeFile in newEpisodes)
            {
                var searchResults = await _detailsApi.SearchTvShows(episodeFile.Name);
                if (searchResults == null || !searchResults.Any())
                {
                    episodeFile.SeriesDetailsId = null;
                    continue;
                }
                
                var actual = new Fastenshtein.Levenshtein(episodeFile.Name);
                var selectedResult = searchResults.OrderBy(sr => actual.DistanceFrom(sr.Name)).First();
                
                var seriesDetails = await functionCache.Invoke(selectedResult.Id, library.Language);
                episodeFile.SeriesDetailsId = seriesDetails.Id;
                allDetails.Add(seriesDetails);
            }

            return allDetails.ToList();
        }
        private Task<SeriesDetails> FetchSeriesDetails(int id, string language) => _detailsApi.FetchTvDetails(id, language);

    }
}