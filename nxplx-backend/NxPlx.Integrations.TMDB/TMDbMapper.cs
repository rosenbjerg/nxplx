using System;
using System.Collections.Generic;
using System.Linq;
using NxPlx.Abstractions;
using NxPlx.Integrations.TMDBApi.Models.Movie;
using NxPlx.Integrations.TMDBApi.Models.Tv;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using CreatedBy = NxPlx.Models.Details.Series.CreatedBy;
using MovieCollection = NxPlx.Models.Details.Film.MovieCollection;
using Network = NxPlx.Models.Details.Series.Network;
using ProductionCountry = NxPlx.Models.Details.Film.ProductionCountry;
using SpokenLanguage = NxPlx.Models.Details.Film.SpokenLanguage;

namespace NxPlx.Integrations.TMDBApi
{
    public class TMDbMapper : MapperBase, IDetailsMapper
    {
        public TMDbMapper()
        {
            SetMapping<TvDetails, SeriesDetails>(tvDetails => new SeriesDetails
            {
                Id = tvDetails.Id,
                BackdropPath = tvDetails.backdrop_path,
                FirstAirDate = tvDetails.first_air_date,
                InProduction = tvDetails.in_production,
                LastAirDate = tvDetails.last_air_date,
                Name = tvDetails.name,
                Overview = tvDetails.overview,
                Popularity = tvDetails.popularity,
                Type = tvDetails.type,
                OriginalLanguage = tvDetails.original_language,
                OriginalName = tvDetails.original_name,
                PosterPath = tvDetails.poster_path,
                VoteAverage = tvDetails.vote_average,
                VoteCount = tvDetails.vote_count,
            
                CreatedBy = tvDetails.created_by.Select(cb => new CreatedBy
                {
                    CreatorId = cb.id,
                    SeriesDetailsId = tvDetails.Id
                }).ToList(),
                Genres = tvDetails.genres.Select(g => new InGenre
                {
                    GenreId = g.id,
                    DetailsEntityId = tvDetails.Id
                }).ToList(),
                Networks = tvDetails.networks.Select(n => new BroadcastOn()
                {
                    NetworkId = n.id,
                    SeriesDetailsId = tvDetails.Id
                }).ToList(),
                Seasons = tvDetails.seasons.Select(s => new Season
                {
                    SeasonDetailsId = s.id,
                    SeriesDetailsId = tvDetails.Id
                }).ToList(),
                ProductionCompanies = tvDetails.production_companies.Select(pb => new ProducedBy()
                {
                    ProductionCompanyId = pb.id,
                    DetailsEntityId = tvDetails.Id
                }).ToList(),
            });
            
            SetMapping<MovieDetails, FilmDetails>(movieDetails => new FilmDetails
            {
                Id = movieDetails.Id,
                BackdropPath = movieDetails.backdrop_path,
                Overview = movieDetails.overview,
                Popularity = movieDetails.popularity,
                OriginalLanguage = movieDetails.original_language,
                PosterPath = movieDetails.poster_path,
                VoteAverage = movieDetails.vote_average,
                VoteCount = movieDetails.vote_count,
                Adult = movieDetails.adult,
                Budget = movieDetails.budget,
                Revenue = movieDetails.revenue,
                Runtime = movieDetails.runtime,
                Tagline = movieDetails.tagline,
                Title = movieDetails.title,
                ImdbId = movieDetails.imdb_id,
                OriginalTitle = movieDetails.original_title,
                ReleaseDate = movieDetails.release_date,
                
                
                BelongsToCollection = movieDetails.belongs_to_collection == null ? null : new BelongsInCollection
                {
                    MovieCollectionId = movieDetails.Id,
                    FilmDetailsId = movieDetails.belongs_to_collection.id
                },
                Genres = movieDetails?.genres?.Select(g => new InGenre
                {
                    GenreId = g.id,
                    DetailsEntityId = movieDetails.Id
                }).ToList(),
                SpokenLanguages = movieDetails?.spoken_languages?.Select(ls => new LanguageSpoken
                {
                    SpokenLanguageId = ls.iso_6391,
                    FilmDetailsId = movieDetails.Id
                }).ToList(),
                ProductionCountries = movieDetails?.production_countries?.Select(pi => new ProducedIn
                {
                    ProductionCountryId = pi.iso_3166_1,
                    FilmDetailsId = movieDetails.Id
                }).ToList(),
                ProductionCompanies = movieDetails?.production_companies?.Select(pb => new ProducedBy()
                {
                    ProductionCompanyId = pb.id,
                    DetailsEntityId = movieDetails.Id
                }).ToList()
            });
            
            SetMapping<Models.Genre, Genre>(genre => new Genre
            {
                Id = genre.id,
                Name = genre.name
            });
            
            SetMapping<Models.ProductionCompany, Genre>(productionCompany => new Genre
            {
                Id = productionCompany.id,
                Name = productionCompany.name
            });
            
            SetMapping<Models.Movie.MovieCollection, MovieCollection>(movieCollection => new MovieCollection
            {
                Id = movieCollection.id,
                Name = movieCollection.name
            });
            SetMapping<Models.Movie.ProductionCountry, ProductionCountry>(productionCountry => new ProductionCountry
            {
                Iso3166_1 = productionCountry.iso_3166_1,
                Name = productionCountry.name
            });
            SetMapping<Models.Movie.SpokenLanguage, SpokenLanguage>(spokenLanguage => new SpokenLanguage
            {
                Iso6391 = spokenLanguage.iso_6391,
                Name = spokenLanguage.name
            });
            
            SetMapping<Models.Tv.Network, Network>(network => new Network
            {
                Id = network.id,
                Name = network.name
            });
            
            SetMapping<Models.Tv.CreatedBy, Creator>(createdBy => new Creator
            {
                Id = createdBy.id,
                Name = createdBy.name
            });
            
            SetMapping<Models.Tv.TvDetailsSeason, SeasonDetails>(tvDetailsSeason => new SeasonDetails
            {
                Id = tvDetailsSeason.id,
                Name = tvDetailsSeason.name,
                Overview = tvDetailsSeason.overview,
                AirDate = tvDetailsSeason.air_date,
                PosterPath = tvDetailsSeason.poster_path,
                SeasonNumber = tvDetailsSeason.season_number
            });
            
        }
        

        
        public List<TPropertyEntity> UniqueProperties<TRootEntity, TPropertyEntity, TPropertyKey>(IEnumerable<TRootEntity> entities, 
            Func<TRootEntity, IEnumerable<TPropertyEntity>> selector, 
            Func<TPropertyEntity, TPropertyKey> keySelector)
            where TRootEntity : class
            where TPropertyEntity : class
        {
            var uniqueProperties = new Dictionary<TPropertyKey, TPropertyEntity>();

            
            foreach (var rootEntity in entities)
            {
                var propertyContent = selector(rootEntity);
                if (propertyContent == null) continue;
                foreach (var propertyEntity in propertyContent)
                {   
                    if (propertyEntity == null) continue;
                    var id = keySelector(propertyEntity);
                    if (id == null) continue;
                    if (uniqueProperties.ContainsKey(id)) continue;
                    uniqueProperties[id] = propertyEntity;
                }
            };
            
            return uniqueProperties.Values.ToList();
        }
        
        public List<TPropertyEntity> UniqueProperties<TRootEntity, TPropertyEntity, TPropertyKey>(IEnumerable<TRootEntity> entities, 
            Func<TRootEntity, TPropertyEntity> selector, 
            Func<TPropertyEntity, TPropertyKey> keySelector)
            where TRootEntity : class
            where TPropertyEntity : class
        {
            var uniqueProperties = new Dictionary<TPropertyKey, TPropertyEntity>();

            foreach (var rootEntity in entities)
            {
                var propertyContent = selector(rootEntity);
                if (propertyContent == null) continue;
                var id = keySelector(propertyContent);
                if (id == null) continue;
                if (uniqueProperties.ContainsKey(id))
                    continue;
                uniqueProperties[id] = propertyContent;
            };
            
            return uniqueProperties.Values.ToList();
        }
    }
}