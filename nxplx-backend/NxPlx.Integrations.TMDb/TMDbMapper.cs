using System.Linq;
using NxPlx.Application.Core;
using NxPlx.Integrations.TMDb.Models.Movie;
using NxPlx.Integrations.TMDb.Models.Search;
using NxPlx.Integrations.TMDb.Models.Tv;
using NxPlx.Integrations.TMDb.Models.TvSeason;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Search;
using NxPlx.Models.Details.Series;
using FilmDetails = NxPlx.Models.Details.Film.FilmDetails;
using MovieCollection = NxPlx.Models.Details.Film.MovieCollection;
using Network = NxPlx.Models.Details.Series.Network;
using ProductionCountry = NxPlx.Models.Details.Film.ProductionCountry;
using SeriesDetails = NxPlx.Models.Details.Series.SeriesDetails;
using SpokenLanguage = NxPlx.Models.Details.Film.SpokenLanguage;

namespace NxPlx.Integrations.TMDb
{
    public class TMDbMapper : MapperBase
    {
        public TMDbMapper()
        {
            SetMapping<TvDetails, SeriesDetails>(tvDetails => new SeriesDetails
            {
                Id = tvDetails.Id,
                BackdropPath = TrimStart(tvDetails.backdrop_path, '/'),
                FirstAirDate = tvDetails.first_air_date,
                InProduction = tvDetails.in_production,
                LastAirDate = tvDetails.last_air_date,
                Name = tvDetails.name,
                Overview = tvDetails.overview,
                Popularity = tvDetails.popularity,
                Type = tvDetails.type,
                OriginalLanguage = tvDetails.original_language,
                OriginalName = tvDetails.original_name,
                PosterPath = TrimStart(tvDetails.poster_path, '/'),
                VoteAverage = tvDetails.vote_average,
                VoteCount = tvDetails.vote_count,
            
                Genres = Map<Models.Genre, Genre>(tvDetails.genres).ToList(),
                CreatedBy = Map<CreatedBy, Creator>(tvDetails.created_by).ToList(),
                Networks = Map<Models.Tv.Network, Network>(tvDetails.networks).ToList(),
                Seasons = Map<TvDetailsSeason, SeasonDetails>(tvDetails.seasons).ToList(),
                ProductionCompanies = Map<Models.ProductionCompany, ProductionCompany>(tvDetails.production_companies).ToList()
            });
            
            SetMapping<MovieDetails, FilmDetails>(movieDetails => new FilmDetails
            {
                Id = movieDetails.Id,
                BackdropPath = TrimStart(movieDetails.backdrop_path, '/'),
                Overview = movieDetails.overview,
                Popularity = movieDetails.popularity,
                OriginalLanguage = movieDetails.original_language,
                PosterPath = TrimStart(movieDetails.poster_path, '/'),
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
                
                Genres = Map<Models.Genre, Genre>(movieDetails.genres).ToList(),
                ProductionCompanies = Map<Models.ProductionCompany, ProductionCompany>(movieDetails.production_companies).ToList(),
                ProductionCountries = Map<Models.Movie.ProductionCountry, ProductionCountry>(movieDetails.production_countries).ToList(),
                SpokenLanguages = Map<Models.Movie.SpokenLanguage, SpokenLanguage>(movieDetails.spoken_languages).ToList(),
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
                Name = productionCompany.name,
                LogoPath = TrimStart(productionCompany.logo_path, '/'),
                OriginCountry = productionCompany.origin_country
            });
            
            SetMapping<Models.Movie.MovieCollection, MovieCollection>(movieCollection => new MovieCollection
            {
                Id = movieCollection.id,
                Name = movieCollection.name,
                BackdropPath = TrimStart(movieCollection.backdrop_path, '/'),
                PosterPath = TrimStart(movieCollection.poster_path, '/')
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
                Name = network.name,
                LogoPath = TrimStart(network.logo_path, '/'),
                OriginCountry = network.origin_country
            });
            
            SetMapping<CreatedBy, Creator>(createdBy => new Creator
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
                PosterPath = TrimStart(tvSeasonDetails.poster_path, '/'),
                SeasonNumber = tvSeasonDetails.season_number,
                Episodes = Map<Episode, EpisodeDetails>(tvSeasonDetails.episodes).ToList()
            });
            SetMapping<TvDetailsSeason, SeasonDetails>(tvSeasonDetails => new SeasonDetails
            {
                Id = tvSeasonDetails.id,
                Name = tvSeasonDetails.name,
                Overview = tvSeasonDetails.overview,
                AirDate = tvSeasonDetails.air_date,
                PosterPath = TrimStart(tvSeasonDetails.poster_path, '/'),
                SeasonNumber = tvSeasonDetails.season_number
            });

            SetMapping<Episode, EpisodeDetails>(episode => new EpisodeDetails
            {
                Id = episode.id,
                Name = episode.name,
                Overview = episode.overview,
                SeasonNumber = episode.season_number,
                AirDate = episode.air_date,
                EpisodeNumber = episode.episode_number,
                ProductionCode = episode.production_code,
                StillPath = TrimStart(episode.still_path, '/'),
                VoteAverage = episode.vote_average,
                VoteCount = episode.vote_count
                
                
            });
            
            SetMapping<SearchResult<TvShowResult>, SeriesResult[]>(searchResult 
                => Map<TvShowResult, SeriesResult>(searchResult.results.Take(10)).ToArray());
            
            SetMapping<SearchResult<MovieResult>, FilmResult[]>(searchResult 
                => Map<MovieResult, FilmResult>(searchResult.results.Take(10)).ToArray());
            
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
                FirstAirDate = tvShowResult.first_air_date,
                Votes = tvShowResult.vote_count
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
                ReleaseDate = movieResult.release_date,
                Votes = movieResult.vote_count
            });
        }

        private static string TrimStart(string input, char trim)
        {
            return string.IsNullOrEmpty(input) ? input : input.Trim(trim);
        }
    }
}