using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Models;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;
using NxPlx.Services.Database;
using MovieCollection = NxPlx.Models.Details.Film.MovieCollection;
using Network = NxPlx.Models.Details.Series.Network;
using ProductionCountry = NxPlx.Models.Details.Film.ProductionCountry;
using SpokenLanguage = NxPlx.Models.Details.Film.SpokenLanguage;

namespace NxPlx.Services.Index
{
    public class Indexer
    {
        private IDetailsApi _detailsApi;
        private IDatabaseMapper _databaseMapper;
        private IDetailsMapper _detailsMapper;
        private ILogger _logger;

        public Indexer(IDetailsApi detailsApi, IDetailsMapper detailsMapper, IDatabaseMapper databaseMapper, ILogger logger)
        {
            _detailsApi = detailsApi;
            _detailsMapper = detailsMapper;
            _databaseMapper = databaseMapper;
            _logger = logger;
        }
        private void AddDownloadTask(List<Task> tasks, DetailsEntityBase detailsEntityBase)
        {      
            tasks.AddRange(new[]
            {
                _detailsApi.DownloadImage("w154", detailsEntityBase.PosterPath),
                _detailsApi.DownloadImage("w342", detailsEntityBase.PosterPath),
                _detailsApi.DownloadImage("w1280", detailsEntityBase.PosterPath)
            });
            
            
            if (detailsEntityBase is FilmDetails filmDetails && filmDetails.BelongsToCollection != null)
            {
                tasks.AddRange(new[]
                {
                    _detailsApi.DownloadImage("w154", filmDetails.BelongsToCollection.PosterPath),
                    _detailsApi.DownloadImage("w342", filmDetails.BelongsToCollection.PosterPath),
                    _detailsApi.DownloadImage("w1280", filmDetails.BelongsToCollection.BackdropPath)
                });
            }
        }
        
        public async Task IndexMovieLibrary(IEnumerable<string> folders)
        {
            var startTime = DateTime.UtcNow;
            folders = new[]
            {
                "\\\\raspberrypi.local\\data\\Media\\Film",
                "\\\\raspberrypi.local\\data\\Media\\Disney",
                "\\\\raspberrypi.local\\data\\Media\\Danske film"
            };
            
            var fileIndexer = new FileIndexer();
            using var ctx = new MediaContext();
            ctx.Database.EnsureCreated();

            var currentFilm = ctx.FilmFiles.Select(e => e.Path).ToHashSet();
            var newFilm = fileIndexer.IndexFilm(currentFilm, folders);
            _logger.Info("Found {NewAmount} new film files in {LibraryFoldersAmount} folder(s) in {ScanTime} seconds", newFilm.Count, folders.Count(), Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
            
            var imageDownloads = new List<Task>(newFilm.Count * 6);
            var uniqueDetails = GetUnique(await FindFilmDetails(newFilm, imageDownloads));
            _logger.Info("Downloaded details for {NewAmount} new film in {DownloadTime} seconds", uniqueDetails.Count, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));
            
            var genres = await GetUniqueNew(uniqueDetails.Where(d => d.Genres != null).SelectMany(d => d.Genres));
            var productionCountries = await GetUniqueNew(uniqueDetails.Where(d => d.ProductionCountries != null).SelectMany(d => d.ProductionCountries), pc => pc.Iso3166_1);
            var spokenLanguages = await GetUniqueNew(uniqueDetails.Where(d => d.SpokenLanguages != null).SelectMany(d => d.SpokenLanguages), sl => sl.Iso639_1);
            var productionCompanies = await GetUniqueNew(uniqueDetails.Where(d => d.ProductionCompanies != null).SelectMany(d => d.ProductionCompanies));
            var movieCollections = await GetUniqueNew(uniqueDetails.Where(d => d.BelongsToCollection != null).Select(d => d.BelongsToCollection));
            
            await ctx.AddRangeAsync(genres);
            await ctx.AddRangeAsync(productionCountries);
            await ctx.AddRangeAsync(spokenLanguages);
            await ctx.AddRangeAsync(productionCompanies);
            await ctx.AddRangeAsync(movieCollections);
            var databaseDetails = uniqueDetails.Select(_databaseMapper.Map<FilmDetails, Models.Database.Film.FilmDetails>);
            var newDetails = await GetNew(databaseDetails, d => d.Id);
            await ctx.AddRangeAsync(newDetails);
            await ctx.AddRangeAsync(newFilm);
            
            await ctx.SaveChangesAsync();
            await Task.WhenAll(imageDownloads);
            _logger.Info("Indexing film in {LibraryFoldersAmount} folders took {Elapsed} seconds", folders.Count(), Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

        }

        private async Task<List<FilmDetails>> FindFilmDetails(List<FilmFile> newFilm, List<Task> imageDownloads)
        {
            var allDetails = new List<FilmDetails>();
            foreach (var filmFile in newFilm)
            {
                var searchResults = await _detailsApi.SearchMovies(filmFile.Title, filmFile.Year);
                if (searchResults == null || !searchResults.Any())
                    searchResults = await _detailsApi.SearchMovies(filmFile.Title, 0);

                if (searchResults == null || !searchResults.Any())
                {
                    filmFile.FilmDetailsId = -1;
                    continue;
                }

                var resultDetails = searchResults[0];
                var filmDetails = await _detailsApi.FetchMovieDetails(resultDetails.Id);
                filmFile.FilmDetailsId = filmDetails.Id;
                allDetails.Add(filmDetails);
                AddDownloadTask(imageDownloads, filmDetails);
            }

            return allDetails;
        }

        public async Task IndexSeriesLibrary(IEnumerable<string> folders)
        {
            var startTime = DateTime.UtcNow;
            folders = new[]
            {
                "\\\\raspberrypi.local\\data\\Media\\Serier"
            };
            
            var fileIndexer = new FileIndexer();
            using var ctx = new MediaContext();
            ctx.Database.EnsureCreated();

            var currentEpisodes = ctx.EpisodeFiles.Select(e => e.Path).ToHashSet();
            var newEpisodes = fileIndexer.IndexEpisodes(currentEpisodes, folders);
            _logger.Info("Found {NewAmount} new episode files in {LibraryFoldersAmount} folder(s) in {ScanTime} seconds", newEpisodes.Count, folders.Count(), Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

            var imageDownloads = new List<Task>(newEpisodes.Count * 3);
            var uniqueDetails = await GetUniqueNew(await FindSeriesDetails(newEpisodes, imageDownloads));
            _logger.Info("Downloaded details for {NewAmount} new series in {DownloadTime} seconds", uniqueDetails.Count, Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

            var genres = await GetUniqueNew(uniqueDetails.Where(d => d.Genres != null).SelectMany(d => d.Genres));
            var networks = await GetUniqueNew(uniqueDetails.Where(d => d.Networks != null).SelectMany(d => d.Networks));
            var seasons = await GetUniqueNew(uniqueDetails.Where(d => d.Seasons != null).SelectMany(d => d.Seasons));
            var creators = await GetUniqueNew(uniqueDetails.Where(d => d.CreatedBy != null).SelectMany(d => d.CreatedBy));
            var productionCompanies = await GetUniqueNew(uniqueDetails.Where(d => d.ProductionCompanies != null).SelectMany(d => d.ProductionCompanies));

            await ctx.AddRangeAsync(newEpisodes);
            var databaseDetails = uniqueDetails.Select(_databaseMapper.Map<SeriesDetails, Models.Database.Series.SeriesDetails>);
            var newDetails = await GetNew(databaseDetails, d => d.Id);
            await ctx.AddRangeAsync(newDetails);
            await ctx.AddRangeAsync(genres);
            await ctx.AddRangeAsync(networks);
            await ctx.AddRangeAsync(seasons);
            await ctx.AddRangeAsync(creators);
            await ctx.AddRangeAsync(productionCompanies);
            
            await ctx.SaveChangesAsync();
            await Task.WhenAll(imageDownloads);
            _logger.Info("Indexing episodes in {LibraryFoldersAmount} folders took {Elapsed} seconds", folders.Count(), Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 3));

            Console.WriteLine(DateTime.UtcNow + ": " + "Saved series");
        }

        private async Task<List<SeriesDetails>> FindSeriesDetails(List<EpisodeFile> newEpisodes, List<Task> imageDownloads)
        {
            var allDetails = new List<SeriesDetails>();

            foreach (var episodeFile in newEpisodes)
            {
                var searchResults = await _detailsApi.SearchTvShows(episodeFile.Name);

                var details = searchResults.Any() ? searchResults[0] : null;

                if (details == null) continue;

                var seriesDetails = await _detailsApi.FetchTvDetails(details.Id);
                episodeFile.SeriesDetailsId = seriesDetails.Id;
                allDetails.Add(seriesDetails);
                AddDownloadTask(imageDownloads, seriesDetails);
            }

            return allDetails;
        }

        private static Task<List<T>> GetUniqueNew<T>(IEnumerable<T> entities)
            where T : EntityBase
            => GetUniqueNew(entities, e => e.Id);
        
        private static async Task<List<T>> GetUniqueNew<T, TKey>(IEnumerable<T> entities, Expression<Func<T, TKey>> keySelector)
            where T : class
        {
            var keySelectorFunc = keySelector.Compile();
            var unique = GetUnique(entities, keySelectorFunc);
            return await GetNew(unique, keySelector);
        }
        
        private static async Task<List<T>> GetNew<T, TKey>(IEnumerable<T> entities, Expression<Func<T, TKey>> keySelector)
            where T : class
        {
            using var ctx = new MediaContext();
            var unique = await ctx.Set<T>().Select(keySelector).ToListAsync();
            var uniqueHashset = unique.ToHashSet();

            var keySelectorFunc = keySelector.Compile();
            return entities.Where(e => !uniqueHashset.Contains(keySelectorFunc(e))).ToList();
        }
        
        private static List<T> GetUnique<T>(IEnumerable<T> entities)
            where T : EntityBase
            => GetUnique(entities, e => e.Id);
        
        private static List<T> GetUnique<T, TKey>(IEnumerable<T> entities, Func<T, TKey> keySelector)
            where T : class
        {
            var unique = new Dictionary<TKey, T>();
            foreach (var entity in entities)
            {
                if (entity == null) 
                    continue;
                var key = keySelector(entity);
                unique[key] = entity;
            }

            return unique.Values.ToList();
        }
    }
}