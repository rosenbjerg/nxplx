using System.Linq;
using AutoMapper;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Film;
using NxPlx.Application.Models.Series;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;

namespace NxPlx.Application.Mapping
{
    public class DtoProfile : Profile
    {
        public DtoProfile()
        {
            CreateMap<DbSeriesDetails, SeriesDto>()
                .ForMember(dst => dst.Networks, opt => opt.MapFrom(src => src.Networks.Select(je => je.Entity2)))
                .ForMember(dst => dst.Genres, opt => opt.MapFrom(src => src.Genres.Select(je => je.Entity2)))
                .ForMember(dst => dst.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy.Select(je => je.Entity2)))
                .ForMember(dst => dst.ProductionCompanies, opt => opt.MapFrom(src => src.ProductionCompanies.Select(je => je.Entity2)));

            CreateMap<SeasonDetails, SeasonDto>()
                .ForMember(dst => dst.Number, opt => opt.MapFrom(src => src.SeasonNumber))
                .ForMember(dst => dst.Episodes, opt => opt.Ignore());

            CreateMap<DbSeriesDetails, OverviewElementDto>()
                .ForMember(dst => dst.Kind, opt => opt.MapFrom(_ => "series"))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Year, opt => opt.MapFrom(src => src.FirstAirDate == null ? 9999 : src.FirstAirDate.Value.Year))
                .ForMember(dst => dst.Genres, opt => opt.MapFrom(src => src.Genres.Select(je => je.Entity2Id).Distinct()));
            
            CreateMap<DbFilmDetails, OverviewElementDto>()
                .ForMember(dst => dst.Kind, opt => opt.MapFrom(_ => "film"))
                .ForMember(dst => dst.Year, opt => opt.MapFrom(src => src.ReleaseDate == null ? 9999 : src.ReleaseDate.Value.Year))
                .ForMember(dst => dst.Genres, opt => opt.MapFrom(src => src.Genres.Select(je => je.Entity2Id)));
            
            
            CreateMap<MovieCollection, OverviewElementDto>()
                .ForMember(dst => dst.Kind, opt => opt.MapFrom(_ => "collection"))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Genres, opt => opt.MapFrom(src => src.Movies.SelectMany(f => f.Genres.Select(g => g.Entity2Id)).Distinct().ToList()))
                .ForMember(dst => dst.Year, opt => opt.MapFrom(src => src.Movies.Min(f => f.ReleaseDate == null ? 9999 : f.ReleaseDate.Value.Year)));

            CreateMap<FilmFile, FilmDto>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.FilmDetailsId))
                .ForMember(dst => dst.Fid, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Library, opt => opt.MapFrom(src => src.PartOfLibraryId))
                .ForMember(dst => dst.BackdropPath, opt => opt.MapFrom(src => src.FilmDetails.BackdropPath))
                .ForMember(dst => dst.BackdropBlurHash, opt => opt.MapFrom(src => src.FilmDetails.BackdropBlurHash))
                .ForMember(dst => dst.PosterPath, opt => opt.MapFrom(src => src.FilmDetails.PosterPath))
                .ForMember(dst => dst.PosterBlurHash, opt => opt.MapFrom(src => src.FilmDetails.PosterBlurHash))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.FilmDetails.Title))
                .ForMember(dst => dst.Budget, opt => opt.MapFrom(src => src.FilmDetails.Budget))
                .ForMember(dst => dst.Genres, opt => opt.MapFrom(src => src.FilmDetails.Genres.Select(je => je.Entity2)))
                .ForMember(dst => dst.Overview, opt => opt.MapFrom(src => src.FilmDetails.Overview))
                .ForMember(dst => dst.Popularity, opt => opt.MapFrom(src => src.FilmDetails.Popularity))
                .ForMember(dst => dst.Revenue, opt => opt.MapFrom(src => src.FilmDetails.Revenue))
                .ForMember(dst => dst.Runtime, opt => opt.MapFrom(src => src.FilmDetails.Runtime))
                .ForMember(dst => dst.Tagline, opt => opt.MapFrom(src => src.FilmDetails.Tagline))
                .ForMember(dst => dst.ImdbId, opt => opt.MapFrom(src => src.FilmDetails.ImdbId))
                .ForMember(dst => dst.OriginalLanguage, opt => opt.MapFrom(src => src.FilmDetails.OriginalLanguage))
                .ForMember(dst => dst.OriginalTitle, opt => opt.MapFrom(src => src.FilmDetails.OriginalTitle))
                .ForMember(dst => dst.ProductionCompanies, opt => opt.MapFrom(src => src.FilmDetails.ProductionCompanies.Select(je => je.Entity2)))
                .ForMember(dst => dst.ProductionCountries, opt => opt.MapFrom(src => src.FilmDetails.ProductionCountries.Select(je => je.Entity2)))
                .ForMember(dst => dst.SpokenLanguages, opt => opt.MapFrom(src => src.FilmDetails.SpokenLanguages.Select(je => je.Entity2)))
                .ForMember(dst => dst.ReleaseDate, opt => opt.MapFrom(src => src.FilmDetails.ReleaseDate))
                .ForMember(dst => dst.VoteAverage, opt => opt.MapFrom(src => src.FilmDetails.VoteAverage))
                .ForMember(dst => dst.VoteCount, opt => opt.MapFrom(src => src.FilmDetails.VoteCount))
                .ForMember(dst => dst.BelongsToCollectionId, opt => opt.MapFrom(src => src.FilmDetails.BelongsInCollectionId));

            CreateMap<FilmFile, InfoDto>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.FilmDetailsId))
                .ForMember(dst => dst.Fid, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Duration, opt => opt.MapFrom(src => src.MediaDetails.Duration))
                .ForMember(dst => dst.BackdropPath, opt => opt.MapFrom(src => src.FilmDetails.BackdropPath))
                .ForMember(dst => dst.BackdropBlurHash, opt => opt.MapFrom(src => src.FilmDetails.BackdropBlurHash))
                .ForMember(dst => dst.PosterPath, opt => opt.MapFrom(src => src.FilmDetails.PosterPath))
                .ForMember(dst => dst.PosterBlurHash, opt => opt.MapFrom(src => src.FilmDetails.PosterBlurHash))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.FilmDetails.Title))
                .ForMember(dst => dst.Subtitles, opt => opt.MapFrom(src => src.Subtitles.Select(s => s.Language)));

            CreateMap<EpisodeFile, EpisodeFileDto>()
                .ForMember(dst => dst.Subtitles, opt => opt.MapFrom(src => src.Subtitles.Select(s => s.Language)));

            CreateMap<EpisodeFile, InfoDto>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.SeriesDetailsId))
                .ForMember(dst => dst.Fid, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Duration, opt => opt.MapFrom(src => src.MediaDetails.Duration))
                .ForMember(dst => dst.BackdropPath, opt => opt.MapFrom(src => src.SeriesDetails.BackdropPath))
                .ForMember(dst => dst.BackdropBlurHash, opt => opt.MapFrom(src => src.SeriesDetails.BackdropBlurHash))
                .ForMember(dst => dst.PosterPath, opt => opt.MapFrom(src => src.SeriesDetails.PosterPath))
                .ForMember(dst => dst.PosterBlurHash, opt => opt.MapFrom(src => src.SeriesDetails.PosterBlurHash))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => $"{src.SeriesDetails.Name} - S{src.SeasonNumber:D2}E{src.EpisodeNumber:D2}"))
                .ForMember(dst => dst.Subtitles, opt => opt.MapFrom(src => src.Subtitles.Select(s => s.Language)));
            
            CreateMap<EpisodeFile, NextEpisodeDto>()
                .ForMember(dst => dst.Fid, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.PosterPath, opt => opt.MapFrom(src => src.SeriesDetails.PosterPath))
                .ForMember(dst => dst.PosterBlurHash, opt => opt.MapFrom(src => src.SeriesDetails.PosterBlurHash))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => $"{src.SeriesDetails.Name} - S{src.SeasonNumber:D2}E{src.EpisodeNumber:D2}"));

            CreateMap<SubtitleFile, SubtitleFileDto>();
            CreateMap<Genre, GenreDto>();
            CreateMap<ProductionCompany, ProductionCompanyDto>();
            
            CreateMap<ProductionCountry, ProductionCountryDto>()
                .ForMember(dst => dst.Iso31661, opt => opt.MapFrom(src => src.Iso3166_1));
            
            CreateMap<SpokenLanguage, SpokenLanguageDto>()
                .ForMember(dst => dst.Iso6391, opt => opt.MapFrom(src => src.Iso639_1));

            CreateMap<MovieCollection, MovieCollectionDto>();

            CreateMap<Network, NetworkDto>();
            CreateMap<Creator, CreatorDto>();

            CreateMap<User, UserDto>()
                .ForMember(dst => dst.Libraries, opt => opt.MapFrom(src => src.LibraryAccessIds));
            
            CreateMap<Library, LibraryDto>()
                .ForMember(dst => dst.Kind, opt => opt.MapFrom(src => src.Kind.ToString().ToLowerInvariant()));
            
            CreateMap<Library, AdminLibraryDto>()
                .ForMember(dst => dst.Kind, opt => opt.MapFrom(src => src.Kind.ToString().ToLowerInvariant()));
            
            CreateMap<UserSession, UserSessionDto>();
        }
    }
}