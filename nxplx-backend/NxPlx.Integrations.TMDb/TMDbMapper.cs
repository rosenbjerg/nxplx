using AutoMapper;
using NxPlx.Integrations.TMDb.Models.Movie;
using NxPlx.Integrations.TMDb.Models.Search;
using NxPlx.Integrations.TMDb.Models.Tv;
using NxPlx.Integrations.TMDb.Models.TvSeason;
using NxPlx.Domain.Models.Database;
using NxPlx.Domain.Models.Details;
using NxPlx.Domain.Models.Details.Search;
using NxPlx.Domain.Models.Details.Series;
using MovieCollection = NxPlx.Domain.Models.Details.Film.MovieCollection;
using Network = NxPlx.Domain.Models.Details.Series.Network;
using ProductionCountry = NxPlx.Domain.Models.Details.Film.ProductionCountry;
using SpokenLanguage = NxPlx.Domain.Models.Details.Film.SpokenLanguage;

namespace NxPlx.Integrations.TMDb
{
    public class TMDbProfile : Profile
    {
        public TMDbProfile()
        {
            CreateMap<TvDetails, DbSeriesDetails>()
                .ForMember(dst => dst.BackdropPath, opt => opt.MapFrom(src => TrimStart(src.backdrop_path, '/')))
                .ForMember(dst => dst.FirstAirDate, opt => opt.MapFrom(src => src.first_air_date))
                .ForMember(dst => dst.InProduction, opt => opt.MapFrom(src => src.in_production))
                .ForMember(dst => dst.LastAirDate, opt => opt.MapFrom(src => src.last_air_date))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(dst => dst.Overview, opt => opt.MapFrom(src => src.overview))
                .ForMember(dst => dst.Popularity, opt => opt.MapFrom(src => src.popularity))
                .ForMember(dst => dst.OriginalLanguage, opt => opt.MapFrom(src => src.original_language))
                .ForMember(dst => dst.OriginalName, opt => opt.MapFrom(src => src.original_name))
                .ForMember(dst => dst.PosterPath, opt => opt.MapFrom(src => TrimStart(src.poster_path, '/')))
                .ForMember(dst => dst.VoteAverage, opt => opt.MapFrom(src => src.vote_average))
                .ForMember(dst => dst.VoteCount, opt => opt.MapFrom(src => src.vote_count))
                .ForMember(dst => dst.Genres, opt => opt.MapFrom(src => src.genres))
                .ForMember(dst => dst.CreatedBy, opt => opt.MapFrom(src => src.created_by))
                .ForMember(dst => dst.Networks, opt => opt.MapFrom(src => src.networks))
                .ForMember(dst => dst.Seasons, opt => opt.MapFrom(src => src.seasons))
                .ForMember(dst => dst.ProductionCompanies, opt => opt.MapFrom(src => src.production_companies));
            
            CreateMap<MovieDetails, DbFilmDetails>()
                .ForMember(dst => dst.BackdropPath, opt => opt.MapFrom(src => TrimStart(src.backdrop_path, '/')))
                .ForMember(dst => dst.Overview, opt => opt.MapFrom(src => src.overview))
                .ForMember(dst => dst.Popularity, opt => opt.MapFrom(src => src.popularity))
                .ForMember(dst => dst.OriginalLanguage, opt => opt.MapFrom(src => src.original_language))
                .ForMember(dst => dst.PosterPath, opt => opt.MapFrom(src => TrimStart(src.poster_path, '/')))
                .ForMember(dst => dst.VoteAverage, opt => opt.MapFrom(src => src.vote_average))
                .ForMember(dst => dst.VoteCount, opt => opt.MapFrom(src => src.vote_count))
                .ForMember(dst => dst.Genres, opt => opt.MapFrom(src => src.genres))
                .ForMember(dst => dst.Adult, opt => opt.MapFrom(src => src.adult))
                .ForMember(dst => dst.Budget, opt => opt.MapFrom(src => src.budget))
                .ForMember(dst => dst.Revenue, opt => opt.MapFrom(src => src.revenue))
                .ForMember(dst => dst.Runtime, opt => opt.MapFrom(src => src.runtime))
                .ForMember(dst => dst.Tagline, opt => opt.MapFrom(src => src.tagline))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.title))
                .ForMember(dst => dst.ImdbId, opt => opt.MapFrom(src => src.imdb_id))
                .ForMember(dst => dst.OriginalTitle, opt => opt.MapFrom(src => src.original_title))
                .ForMember(dst => dst.ReleaseDate, opt => opt.MapFrom(src => src.release_date))
                .ForMember(dst => dst.ProductionCountries, opt => opt.MapFrom(src => src.production_countries))
                .ForMember(dst => dst.SpokenLanguages, opt => opt.MapFrom(src => src.spoken_languages))
                .ForMember(dst => dst.BelongsInCollection, opt => opt.MapFrom(src => src.belongs_to_collection))
                .ForMember(dst => dst.ProductionCompanies, opt => opt.MapFrom(src => src.production_companies));


            CreateMap<Models.Genre, Genre>();
            
            CreateMap<Models.ProductionCompany, ProductionCompany>()
                .ForMember(dst => dst.LogoPath, opt => opt.MapFrom(src => TrimStart(src.logo_path, '/')));
            
            CreateMap<Models.Movie.MovieCollection, MovieCollection>()
                .ForMember(dst => dst.PosterPath, opt => opt.MapFrom(src => TrimStart(src.poster_path, '/')))
                .ForMember(dst => dst.BackdropPath, opt => opt.MapFrom(src => TrimStart(src.backdrop_path, '/')));

            CreateMap<Models.Movie.ProductionCountry, ProductionCountry>()
                .ForMember(dst => dst.Iso3166_1, opt => opt.MapFrom(src => src.iso_3166_1));

            CreateMap<Models.Movie.SpokenLanguage, SpokenLanguage>()
                .ForMember(dst => dst.Iso639_1, opt => opt.MapFrom(src => src.iso_639_1));
            

            CreateMap<Models.Tv.Network, Network>()
                .ForMember(dst => dst.LogoPath, opt => opt.MapFrom(src => TrimStart(src.logo_path, '/')));
            
            
            CreateMap<CreatedBy, Creator>();

            CreateMap<TvSeasonDetails, SeasonDetails>()
                .ForMember(dst => dst.PosterPath, opt => opt.MapFrom(src => TrimStart(src.poster_path, '/')))
                .ForMember(dst => dst.AirDate, opt => opt.MapFrom(src => src.air_date))
                .ForMember(dst => dst.SeasonNumber, opt => opt.MapFrom(src => src.season_number))
                .ForMember(dst => dst.Episodes, opt => opt.MapFrom(src => src.episodes));
            
            CreateMap<TvDetailsSeason, SeasonDetails>()
                .ForMember(dst => dst.PosterPath, opt => opt.MapFrom(src => TrimStart(src.poster_path, '/')))
                .ForMember(dst => dst.AirDate, opt => opt.MapFrom(src => src.air_date))
                .ForMember(dst => dst.SeasonNumber, opt => opt.MapFrom(src => src.season_number));
            
            CreateMap<Episode, EpisodeDetails>()
                .ForMember(dst => dst.StillPath, opt => opt.MapFrom(src => TrimStart(src.still_path, '/')))
                .ForMember(dst => dst.AirDate, opt => opt.MapFrom(src => src.air_date))
                .ForMember(dst => dst.EpisodeNumber, opt => opt.MapFrom(src => src.episode_number))
                .ForMember(dst => dst.ProductionCode, opt => opt.MapFrom(src => src.production_code))
                .ForMember(dst => dst.VoteAverage, opt => opt.MapFrom(src => src.vote_average))
                .ForMember(dst => dst.VoteCount, opt => opt.MapFrom(src => src.vote_count))
                .ForMember(dst => dst.SeasonNumber, opt => opt.MapFrom(src => src.season_number));

            CreateMap<TvShowResult, SeriesResult>()
                .ForMember(dst => dst.GenreIds, opt => opt.MapFrom(src => src.genre_ids))
                .ForMember(dst => dst.OriginalLanguage, opt => opt.MapFrom(src => src.original_language))
                .ForMember(dst => dst.OriginalName, opt => opt.MapFrom(src => src.original_name))
                .ForMember(dst => dst.OriginCountry, opt => opt.MapFrom(src => src.origin_country))
                .ForMember(dst => dst.FirstAirDate, opt => opt.MapFrom(src => src.first_air_date))
                .ForMember(dst => dst.Votes, opt => opt.MapFrom(src => src.vote_count));

            CreateMap<MovieResult, FilmResult>()
                .ForMember(dst => dst.GenreIds, opt => opt.MapFrom(src => src.genre_ids))
                .ForMember(dst => dst.OriginalLanguage, opt => opt.MapFrom(src => src.original_language))
                .ForMember(dst => dst.ReleaseDate, opt => opt.MapFrom(src => src.release_date))
                .ForMember(dst => dst.Votes, opt => opt.MapFrom(src => src.vote_count));
        }

        private static string TrimStart(string input, char trim)
        {
            return string.IsNullOrEmpty(input) ? input : input.Trim(trim);
        }
    }
}