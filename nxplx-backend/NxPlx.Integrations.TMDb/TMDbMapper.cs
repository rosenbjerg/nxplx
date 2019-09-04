using System;
using System.Collections.Generic;
using System.Linq;
using NxPlx.Abstractions;
using NxPlx.Integrations.TMDBApi.Models.Movie;
using NxPlx.Integrations.TMDBApi.Models.Search;
using NxPlx.Integrations.TMDBApi.Models.Tv;
using NxPlx.Integrations.TMDBApi.Models.TvSeason;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Search;
using NxPlx.Models.Details.Series;
using FilmDetails = NxPlx.Models.Details.Film.FilmDetails;
using MovieCollection = NxPlx.Models.Details.Film.MovieCollection;
using Network = NxPlx.Models.Details.Series.Network;
using ProductionCountry = NxPlx.Models.Details.Film.ProductionCountry;
using SeriesDetails = NxPlx.Models.Details.Series.SeriesDetails;
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
            
                CreatedBy = tvDetails.created_by?.Select(Map<Models.Tv.CreatedBy, Creator>).ToList(),
                Networks = tvDetails.networks?.Select(Map<Models.Tv.Network, Network>).ToList(),
                Seasons = tvDetails.seasons?.Select(Map<TvDetailsSeason, SeasonDetails>).ToList(),
                ProductionCompanies = tvDetails.production_companies?.Select(Map<Models.ProductionCompany, ProductionCompany>).ToList(),
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
                
                Genres = movieDetails.genres?.Select(Map<Models.Genre, Genre>).ToList(),
                ProductionCompanies = movieDetails.production_companies?.Select(Map<Models.ProductionCompany, ProductionCompany>).ToList(),
                ProductionCountries = movieDetails.production_countries?.Select(Map<Models.Movie.ProductionCountry, ProductionCountry>).ToList(),
                SpokenLanguages = movieDetails.spoken_languages?.Select(Map<Models.Movie.SpokenLanguage, SpokenLanguage>).ToList(),
                BelongsToCollection = Map<Models.Movie.MovieCollection, MovieCollection>(movieDetails.belongs_to_collection),
            });
            
            SetMapping<Models.Genre, Genre>(genre => new Genre
            {
                Id = genre.id,
                Name = genre.name
            });
            
            SetMapping<Models.ProductionCompany, ProductionCompany>(productionCompany => new ProductionCompany
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
                Iso639_1 = spokenLanguage.iso_639_1,
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
            
            SetMapping<TvSeasonDetails, SeasonDetails>(tvSeasonDetails => new SeasonDetails
            {
                Id = tvSeasonDetails.id,
                Name = tvSeasonDetails.name,
                Overview = tvSeasonDetails.overview,
                AirDate = tvSeasonDetails.air_date,
                PosterPath = tvSeasonDetails.poster_path,
                SeasonNumber = tvSeasonDetails.season_number,
                Episodes = tvSeasonDetails.episodes.Select(Map<Episode, EpisodeDetails>).ToList()
            });
            
            SetMapping<Episode, EpisodeDetails>(episode => new EpisodeDetails
            {
                Id = episode.id,
                Name = episode.name,
                Overview = episode.overview,
                SeasonNumber = episode.season_number,
                
            });
            
            SetMapping<SearchResult<TvShowResult>, SeriesResult[]>(searchResult 
                => searchResult.results?.Select(Map<TvShowResult, SeriesResult>).ToArray());
            
            SetMapping<SearchResult<MovieResult>, FilmResult[]>(searchResult 
                => searchResult.results?.Select(Map<MovieResult, FilmResult>).ToArray());
            
            SetMapping<TvShowResult, SeriesResult>(tvShowResult => new SeriesResult
            {
                Id = tvShowResult.id,
                Overview = tvShowResult.overview,
                Popularity = tvShowResult.popularity,
                GenreIds = tvShowResult.genre_ids,
                OriginalLanguage = tvShowResult.original_language,
                Name = tvShowResult.name,
                OriginalName = tvShowResult.original_name,
                OriginCountry = tvShowResult.origin_country,
                FirstAirDate = tvShowResult.first_air_date
            });
            
            SetMapping<MovieResult, FilmResult>(movieResult => new FilmResult
            {
                Id = movieResult.id,
                Overview = movieResult.overview,
                Popularity = movieResult.popularity,
                Title = movieResult.title,
                GenreIds = movieResult.genre_ids,
                OriginalLanguage = movieResult.original_language,
                OriginalTitle = movieResult.original_title,
                ReleaseDate = movieResult.release_date
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