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
            folders = new[]
            {
                "\\\\raspberrypi.local\\data\\Media\\Film",
                "\\\\raspberrypi.local\\data\\Media\\Disney",
                "\\\\raspberrypi.local\\data\\Media\\Danske film"
            };
            
            var fileIndexer = new FileIndexer();
            using var ctx = new MediaContext();
            ctx.Database.EnsureCreated();

            Console.WriteLine(DateTime.UtcNow + ": " + "Fetching current film");
            var currentFilm = ctx.FilmFiles.Select(e => e.Path).ToHashSet();
            Console.WriteLine(DateTime.UtcNow + ": " + "Indexing film");
            var newFilm = fileIndexer.IndexFilm(currentFilm, folders);


            Console.WriteLine(DateTime.UtcNow + ": " + "Fetching TMDb details and posters");
            var imageDownloads = new List<Task>(newFilm.Count * 6);
            var allDetails = new List<FilmDetails>();
                
            foreach (var filmFile in newFilm)
            {
                var searchResults = await _detailsApi.SearchMovies(filmFile.Title, filmFile.Year);
                if (searchResults == null || !searchResults.Any()) 
                    searchResults = await _detailsApi.SearchMovies(filmFile.Title, 0);

                if (searchResults == null || !searchResults.Any()) 
                    continue;
                
                var resultDetails = searchResults[0];
                var filmDetails = await _detailsApi.FetchMovieDetails(resultDetails.Id);
                filmFile.FilmDetailsId = filmDetails.Id;
                allDetails.Add(filmDetails);
                AddDownloadTask(imageDownloads, filmDetails);
            }


            Console.WriteLine(DateTime.UtcNow + ": " + "Saving unique subentities");
            var uniqueDetails = GetUnique(allDetails)
                .Select(_databaseMapper.Map<FilmDetails, Models.Database.Film.FilmDetails>)
                .ToList();
            var genres = GetUnique(allDetails.SelectMany(d => d.Genres ?? new List<Genre>()));
            var productionCountries = GetUnique(allDetails.SelectMany(d => d.ProductionCountries ?? new List<ProductionCountry>()), pc => pc.Iso3166_1);
            var spokenLanguages = GetUnique(allDetails.SelectMany(d => d.SpokenLanguages ?? new List<SpokenLanguage>()), sl => sl.Iso639_1);
            var productionCompanies = GetUnique(allDetails.SelectMany(d => d.ProductionCompanies ?? new List<ProductionCompany>()));
            var movieCollections = GetUnique(allDetails.Select(d => d.BelongsToCollection));
            
            Console.WriteLine(DateTime.UtcNow + ": " + "Await image downloads to finish");
            await Task.WhenAll(imageDownloads);


            Console.WriteLine(DateTime.UtcNow + ": " + "Determining deleted film");
            var deletedFilm = currentFilm
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount / 2)
                .Where(cf => !File.Exists(cf));
                
            Console.WriteLine(DateTime.UtcNow + ": " + "Tracking film");
            await ctx.AddRangeAsync(newFilm);
            await ctx.AddRangeAsync(uniqueDetails);
            await ctx.AddRangeAsync(genres);
            await ctx.AddRangeAsync(productionCountries);
            await ctx.AddRangeAsync(spokenLanguages);
            await ctx.AddRangeAsync(productionCompanies);
            await ctx.AddRangeAsync(movieCollections);
            
            Console.WriteLine(DateTime.UtcNow + ": " + "Saving film");
            await ctx.SaveChangesAsync();
            Console.WriteLine(DateTime.UtcNow + ": " + "Saved film");
        }
        public async Task IndexSeriesLibrary(IEnumerable<string> folders)
        {
            folders = new[]
            {
                "\\\\raspberrypi.local\\data\\Media\\Serier"
            };
            
            var fileIndexer = new FileIndexer();
            using var ctx = new MediaContext();
            ctx.Database.EnsureCreated();

            Console.WriteLine(DateTime.UtcNow + ": " + "Fetching current series");
            var currentEpisodes = ctx.EpisodeFiles.Select(e => e.Path).ToHashSet();
            Console.WriteLine(DateTime.UtcNow + ": " + "Indexing series");
            var newEpisodes = fileIndexer.IndexEpisodes(currentEpisodes, folders);


            Console.WriteLine(DateTime.UtcNow + ": " + "Fetching TMDb details and posters");
                
            var imageDownloads = new List<Task>(newEpisodes.Count * 3);
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

            Console.WriteLine(DateTime.UtcNow + ": " + "Saving unique subentities");
            var uniqueDetails = GetUnique(allDetails)
                .Select(_databaseMapper.Map<SeriesDetails, Models.Database.Series.SeriesDetails>)
                .ToList();
            var genres = GetUnique(allDetails.SelectMany(d => d.Genres));
            var networks = GetUnique(allDetails.SelectMany(d => d.Networks));
            var seasons = GetUnique(allDetails.SelectMany(d => d.Seasons));
            var productionCompanies = GetUnique(allDetails.SelectMany(d => d.ProductionCompanies));
            var creators = GetUnique(allDetails.SelectMany(d => d.CreatedBy));
       
            
            Console.WriteLine(DateTime.UtcNow + ": " + "Await image downloads to finish");
            await Task.WhenAll(imageDownloads);


            Console.WriteLine(DateTime.UtcNow + ": " + "Determining deleted series");
            var deletedEpisodes = currentEpisodes
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount / 2)
                .Where(cf => !File.Exists(cf));


                
            Console.WriteLine(DateTime.UtcNow + ": " + "Tracking series");
            try
            {
                await ctx.EpisodeFiles.AddRangeAsync(newEpisodes);
                Console.WriteLine(DateTime.UtcNow + ": " + "Saving series");
                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Console.WriteLine(DateTime.UtcNow + ": " + "Saved series");
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