using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Integrations.TMDBApi;
using NxPlx.Integrations.TMDBApi.Models.Movie;
using NxPlx.Integrations.TMDBApi.Models.Tv;
using NxPlx.Integrations.TMDBApi.Models.TvSeason;
using NxPlx.Models;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;
using NxPlx.Services.Database;
using CreatedBy = NxPlx.Integrations.TMDBApi.Models.Tv.CreatedBy;
using Genre = NxPlx.Integrations.TMDBApi.Models.Genre;
using MovieCollection = NxPlx.Models.Details.Film.MovieCollection;
using Network = NxPlx.Models.Details.Series.Network;
using ProductionCountry = NxPlx.Models.Details.Film.ProductionCountry;
using SpokenLanguage = NxPlx.Models.Details.Film.SpokenLanguage;

namespace NxPlx.Services.Index
{
    public class Indexer
    {
        public async Task IndexMovieLibrary(TmdbApi tmdb)
        {
            
            
            var fileIndexer = new FileIndexer();
            using var ctx = new MediaContext();
            ctx.Database.EnsureCreated();

            Console.WriteLine(DateTime.UtcNow + ": " + "Fetching current film");
            var currentFilm = ctx.FilmFiles.Select(e => e.Path).ToHashSet();
            Console.WriteLine(DateTime.UtcNow + ": " + "Indexing film");
            var newFilm = fileIndexer.IndexFilm(currentFilm, "\\\\raspberrypi.local\\data\\Media\\Film",
                "\\\\raspberrypi.local\\data\\Media\\Disney", "\\\\raspberrypi.local\\data\\Media\\Danske film");


            Console.WriteLine(DateTime.UtcNow + ": " + "Fetching TMDb details and posters");

            var mapper = new TMDbMapper();
            var imageDownloads = new List<Task>(newFilm.Count * 6);
            var allDetails = new List<MovieDetails>();
                
            foreach (var filmFile in newFilm)
            {
                var searchResult = await tmdb.SearchMovies(filmFile.Title, filmFile.Year);

                if (searchResult.results == null || !searchResult.results.Any())
                {
                    searchResult = await tmdb.SearchMovies(filmFile.Title, 0);
                }

                var resultDetails = (searchResult.results != null && searchResult.results.Any())
                    ? searchResult.results[0]
                    : null;

                if (resultDetails != null)
                {
                    var movieDetails = await tmdb.FetchMovieDetails(resultDetails.id);
                    filmFile.FilmDetails = mapper.Map<MovieDetails, FilmDetails>(movieDetails);
                    allDetails.Add(movieDetails);

                    imageDownloads.AddRange(new[]
                    {
                        tmdb.DownloadImage("w154", movieDetails.poster_path),
                        tmdb.DownloadImage("w342", movieDetails.poster_path),
                        tmdb.DownloadImage("w1280", movieDetails.backdrop_path)
                    });

                    if (movieDetails.belongs_to_collection != null)
                    {
                        imageDownloads.AddRange(new[]
                        {
                            tmdb.DownloadImage("w154", movieDetails.belongs_to_collection.poster_path),
                            tmdb.DownloadImage("w342", movieDetails.belongs_to_collection.poster_path),
                            tmdb.DownloadImage("w1280", movieDetails.belongs_to_collection.backdrop_path)
                        });
                    }
                        
                }
                else
                {
                    filmFile.FilmDetails = new FilmDetails
                    {
                        Title = filmFile.Title,
                        ReleaseDate = new DateTime(filmFile.Year, 1, 1)
                    };
                }
            }


            Console.WriteLine(DateTime.UtcNow + ": " + "Saving unique subentities");
            await SaveNewUnique<Models.Details.Genre, MovieDetails, Genre, int>(mapper, allDetails, f => f.genres, g => g.id);
            await SaveNewUnique<SpokenLanguage, MovieDetails, Integrations.TMDBApi.Models.Movie.SpokenLanguage, string, string>(mapper, allDetails, f => f.spoken_languages, g => g.iso_6391, g => g.Iso6391);
            await SaveNewUnique<ProductionCountry, MovieDetails, Integrations.TMDBApi.Models.Movie.ProductionCountry, string, string>(mapper, allDetails, f => f.production_countries, g => g.iso_3166_1, g => g.Iso3166_1);
            await SaveNewUnique<ProductionCompany, MovieDetails, Integrations.TMDBApi.Models.ProductionCompany, int>(mapper, allDetails, f => f.production_companies, g => g.id);
            await SaveNewUnique<MovieCollection, MovieDetails, Integrations.TMDBApi.Models.Movie.MovieCollection, int>(mapper, allDetails, f => f.belongs_to_collection, g => g.id);

            Console.WriteLine(DateTime.UtcNow + ": " + "Await image downloads to finish");
            await Task.WhenAll(imageDownloads);


            Console.WriteLine(DateTime.UtcNow + ": " + "Determining deleted film");
            var deletedFilm = currentFilm
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount / 2)
                .Where(cf => !File.Exists(cf));
                
            Console.WriteLine(DateTime.UtcNow + ": " + "Tracking film");
            await ctx.FilmFiles.AddRangeAsync(newFilm);
            Console.WriteLine(DateTime.UtcNow + ": " + "Saving film");
            await ctx.SaveChangesAsync();
            Console.WriteLine(DateTime.UtcNow + ": " + "Saved film");
        }
        public async Task IndexSeriesLibrary(TmdbApi tmdb)
        {
            var fileIndexer = new FileIndexer();
            using var ctx = new MediaContext();
            ctx.Database.EnsureCreated();

            Console.WriteLine(DateTime.UtcNow + ": " + "Fetching current series");
            var currentEpisodes = ctx.EpisodeFiles.Select(e => e.Path).ToHashSet();
            Console.WriteLine(DateTime.UtcNow + ": " + "Indexing series");
            var newEpisodes = fileIndexer.IndexEpisodes(currentEpisodes, "\\\\raspberrypi.local\\data\\Media\\Serier");


            Console.WriteLine(DateTime.UtcNow + ": " + "Fetching TMDb details and posters");
                
            var mapper = new TMDbMapper();
            var imageDownloads = new List<Task>(newEpisodes.Count * 3);
            var allDetails = new List<TvDetails>();
                
            foreach (var episodeFile in newEpisodes)
            {
                var searchResult = await tmdb.SearchTvShows(episodeFile.Name);

                var details = (searchResult.results != null && searchResult.results.Any())
                    ? searchResult.results[0]
                    : null;

                if (details != null)
                {
                    var tvDetails = await tmdb.FetchTvDetails(details.id);
                    episodeFile.SeriesDetails = mapper.Map<TvDetails, SeriesDetails>(tvDetails);
                    allDetails.Add(tvDetails);
                        
                    imageDownloads.AddRange(new[]
                    {
                        tmdb.DownloadImage("w154", details.poster_path),
                        tmdb.DownloadImage("w342", details.poster_path),
                        tmdb.DownloadImage("w1280", details.backdrop_path)
                    });
                }
                else
                {
                    episodeFile.SeriesDetails = new SeriesDetails
                    {
                        Name = episodeFile.Name
                    };
                }
            }

            Console.WriteLine(DateTime.UtcNow + ": " + "Saving unique subentities");
            await SaveNewUnique<SeriesDetails, EpisodeFile, SeriesDetails, int>(mapper, newEpisodes, f => f.SeriesDetails, g => g.Id);
            
            await SaveNewUnique<Models.Details.Genre, TvDetails, Genre, int>(mapper, allDetails, f => f.genres, g => g.id);
            await SaveNewUnique<Creator, TvDetails, CreatedBy, int>(mapper, allDetails, f => f.created_by, g => g.id);
            await SaveNewUnique<Network, TvDetails, Integrations.TMDBApi.Models.Tv.Network, int>(mapper, allDetails, f => f.networks, g => g.id);
            await SaveNewUnique<ProductionCompany, TvDetails, Integrations.TMDBApi.Models.ProductionCompany, int>(mapper, allDetails, f => f.production_companies, g => g.id);
            await SaveNewUnique<SeasonDetails, TvDetails, TvDetailsSeason, int>(mapper, allDetails, f => f.seasons, g => g.id);

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

        private Task SaveNewUnique<TPropertyEntity, TEntity, TProperty, TPropertyKey>(IDetailsMapper mapper, 
            IEnumerable<TEntity> entities, 
            Func<TEntity, TProperty> propertySelector,
            Expression<Func<TProperty, TPropertyKey>> keySelector)
            where TEntity : EntityBase
            where TProperty : class
            where TPropertyEntity : EntityBase
        {
            var keySelectorFunc = keySelector.Compile();
            var properties = mapper.UniqueProperties(entities, propertySelector, keySelectorFunc)
                .Select(mapper.Map<TProperty, TPropertyEntity>);
            return SaveNew(properties);
        }

        private Task SaveNewUnique<TPropertyEntity, TEntity, TProperty, TPropertyKey>(IDetailsMapper mapper,
            IEnumerable<TEntity> entities,
            Func<TEntity, IEnumerable<TProperty>> propertySelector,
            Expression<Func<TProperty, TPropertyKey>> keySelector)
            where TEntity : EntityBase
            where TProperty : class
            where TPropertyEntity : EntityBase
        => SaveNewUnique<TPropertyEntity, TEntity, TProperty, TPropertyKey, int>(mapper, entities, propertySelector, keySelector, pe => pe.Id);

        private Task SaveNewUnique<TPropertyEntity, TEntity, TProperty, TPropertyKey, TPropertyEntityKey>(
            IDetailsMapper mapper, 
            IEnumerable<TEntity> entities, 
            Func<TEntity, IEnumerable<TProperty>> propertySelector,
            Expression<Func<TProperty, TPropertyKey>> keySelector,
            Expression<Func<TPropertyEntity, TPropertyEntityKey>> entityKeySelector)
            where TEntity : EntityBase
            where TProperty : class
            where TPropertyEntity : class
        {
            var keySelectorFunc = keySelector.Compile();
            var properties = mapper.UniqueProperties(entities, propertySelector, keySelectorFunc)
                .Select(mapper.Map<TProperty, TPropertyEntity>);
            return SaveNew(properties, entityKeySelector);
        }

        private Task SaveNew<TPropertyEntity>(IEnumerable<TPropertyEntity> properties)
            where TPropertyEntity : EntityBase
        => SaveNew(properties, pe => pe.Id);
        
        private async Task SaveNew<TPropertyEntity, TPropertyEntityKey>(IEnumerable<TPropertyEntity> properties,
            Expression<Func<TPropertyEntity, TPropertyEntityKey>> entityKeySelector)
            where TPropertyEntity : class
        {
            using var ctx = new MediaContext();
            var existing = await ctx.Set<TPropertyEntity>().Select(entityKeySelector).ToListAsync();
            var existingHashset = existing.ToHashSet();

            var keySelector = entityKeySelector.Compile();
            var newProperties = properties.Where(p => p != null && !existingHashset.Contains(keySelector(p)));

            await ctx.AddRangeAsync(newProperties);
            await ctx.SaveChangesAsync();
        }
    }
}