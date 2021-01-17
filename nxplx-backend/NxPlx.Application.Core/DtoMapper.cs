using System.Linq;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Film;
using NxPlx.Application.Models.Series;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;

namespace NxPlx.Application.Core
{
    public class DtoMapper : MapperBase, IDtoMapper
    {
        public DtoMapper()
        {
            SetMapping<DbSeriesDetails, SeriesDto>(seriesDetails => new SeriesDto
            {
                Id = seriesDetails.Id,
                BackdropPath = seriesDetails.BackdropPath,
                BackdropBlurHash = seriesDetails.BackdropBlurHash,
                PosterPath = seriesDetails.PosterPath,
                PosterBlurHash = seriesDetails.PosterBlurHash,
                VoteAverage = seriesDetails.VoteAverage,
                VoteCount = seriesDetails.VoteCount,
                Name = seriesDetails.Name,
                Networks = Map<Network, NetworkDto>(seriesDetails.Networks).ToList(),
                Genres = Map<Genre, GenreDto>(seriesDetails.Genres).ToList(),
                CreatedBy = Map<Creator, CreatorDto>(seriesDetails.CreatedBy).ToList(),
                ProductionCompanies = Map<ProductionCompany, ProductionCompanyDto>(seriesDetails.ProductionCompanies).ToList(),
                Overview = seriesDetails.Overview
            });
            SetMapping<SeasonDetails, SeasonDto>(seasonDetails => new SeasonDto
            {
                Name = seasonDetails.Name,
                Number = seasonDetails.SeasonNumber,
                AirDate = seasonDetails.AirDate,
                PosterPath = seasonDetails.PosterPath,
                PosterBlurHash = seasonDetails.PosterBlurHash,
                Overview = seasonDetails.Overview,
            });
            SetMapping<DbSeriesDetails, OverviewElementDto>(seriesDetails => new OverviewElementDto
            {
                Id = seriesDetails.Id,
                Kind = "series",
                PosterPath = seriesDetails.PosterPath,
                PosterBlurHash = seriesDetails.PosterBlurHash,
                Title = seriesDetails.Name,
                Genres = seriesDetails.Genres.Select(g => g.Id).Distinct().ToList(),
                Year = seriesDetails.FirstAirDate == null ? 9999 : seriesDetails.FirstAirDate.Value.Year
            });
            SetMapping<DbFilmDetails, OverviewElementDto>(filmDetails => new OverviewElementDto
            {
                Id = filmDetails.Id,
                Kind = "film",
                PosterPath = filmDetails.PosterPath,
                PosterBlurHash = filmDetails.PosterBlurHash,
                Title = filmDetails.Title,
                Genres = filmDetails.Genres.Select(g => g.Id).ToList(),
                Year = filmDetails.ReleaseDate == null ? 9999 : filmDetails.ReleaseDate.Value.Year
            });
            SetMapping<MovieCollection, OverviewElementDto>(movieCollection => new OverviewElementDto
            {
                Id = movieCollection.Id,
                Kind = "collection",
                PosterPath = movieCollection.PosterPath,
                PosterBlurHash = movieCollection.PosterBlurHash,
                Title = movieCollection.Name,
                Genres = movieCollection.Movies.SelectMany(f => f.Genres).Select(g => g.Id).Distinct().ToList(),
                Year = movieCollection.Movies.Min(f => f.ReleaseDate == null ? 9999 : f.ReleaseDate.Value.Year)
            });
            SetMapping<(WatchingProgress wp, EpisodeFile ef), ContinueWatchingDto>(pair => new ContinueWatchingDto
            {
                FileId = pair.ef.Id,
                Kind = "series",
                PosterPath = pair.ef.SeasonDetails.PosterPath ?? pair.ef.SeriesDetails.PosterPath,
                PosterBlurHash = pair.ef.SeasonDetails.PosterBlurHash ?? pair.ef.SeriesDetails.PosterBlurHash,
                Title = $"{(pair.ef.SeriesDetails == null ? "" : pair.ef.SeriesDetails.Name)} - {pair.ef.GetNumber()} - {(pair.ef.EpisodeDetails == null ? "" : pair.ef.EpisodeDetails.Name)}",
                Watched = pair.wp.LastWatched,
                Progress = pair.wp.Time / pair.ef.MediaDetails.Duration
            });
            SetMapping<(WatchingProgress wp, FilmFile ff), ContinueWatchingDto>(pair => new ContinueWatchingDto
            {
                FileId = pair.ff.Id,
                Kind = "film",
                PosterPath = pair.ff.FilmDetails.PosterPath,
                PosterBlurHash = pair.ff.FilmDetails.PosterBlurHash,
                Title = $"{(pair.ff.FilmDetails == null ? "" : pair.ff.FilmDetails.Title)}",
                Watched = pair.wp.LastWatched,
                Progress = pair.wp.Time / pair.ff.MediaDetails.Duration
            });

            SetMapping<FilmFile, FilmDto>(filmFilm => new FilmDto
            {
                Id = filmFilm.FilmDetails.Id,
                Fid = filmFilm.Id,
                Library = filmFilm.PartOfLibraryId,
                BackdropPath = filmFilm.FilmDetails.BackdropPath,
                BackdropBlurHash = filmFilm.FilmDetails.BackdropBlurHash,
                PosterPath = filmFilm.FilmDetails.PosterPath,
                PosterBlurHash = filmFilm.FilmDetails.PosterBlurHash,
                Title = filmFilm.FilmDetails.Title,
                Budget = filmFilm.FilmDetails.Budget,
                Genres = Map<Genre, GenreDto>(filmFilm.FilmDetails.Genres).ToList(),
                Overview = filmFilm.FilmDetails.Overview,
                Popularity = filmFilm.FilmDetails.Popularity,
                Revenue = filmFilm.FilmDetails.Revenue,
                Runtime = filmFilm.FilmDetails.Runtime,
                Tagline = filmFilm.FilmDetails.Tagline,
                ImdbId = filmFilm.FilmDetails.ImdbId,
                OriginalLanguage = filmFilm.FilmDetails.OriginalLanguage,
                OriginalTitle = filmFilm.FilmDetails.OriginalTitle,
                ProductionCompanies = Map<ProductionCompany, ProductionCompanyDto>(filmFilm.FilmDetails.ProductionCompanies).ToList(),
                ProductionCountries = Map<ProductionCountry, ProductionCountryDto>(filmFilm.FilmDetails.ProductionCountries).ToList(),
                ReleaseDate = filmFilm.FilmDetails.ReleaseDate,
                SpokenLanguages = Map<SpokenLanguage, SpokenLanguageDto>(filmFilm.FilmDetails.SpokenLanguages).ToList(),
                VoteAverage = filmFilm.FilmDetails.VoteAverage,
                VoteCount = filmFilm.FilmDetails.VoteCount,
                BelongsToCollectionId = filmFilm.FilmDetails.BelongsInCollectionId
            });
            
            SetMapping<EpisodeFile, EpisodeFileDto>(episodeFile => new EpisodeFileDto
            {
                Id = episodeFile.Id,
                EpisodeNumber = episodeFile.EpisodeNumber,
                SeasonNumber = episodeFile.SeasonNumber,
                Subtitles = episodeFile.Subtitles.Select(s => s.Language)
            });
            SetMapping<EpisodeFile, InfoDto>(episodeFile => new InfoDto
            {
                Id = episodeFile.Id,
                Duration = episodeFile.MediaDetails.Duration,
                BackdropPath = episodeFile.SeriesDetails.BackdropPath,
                BackdropBlurHash = episodeFile.SeriesDetails.BackdropBlurHash,
                PosterPath = episodeFile.SeriesDetails.PosterPath,
                PosterBlurHash = episodeFile.SeriesDetails.PosterBlurHash,
                Title = $"{episodeFile.SeriesDetails.Name} - S{episodeFile.SeasonNumber:D2}E{episodeFile.EpisodeNumber:D2} - {episodeFile.EpisodeDetails.Name}",
                Subtitles = episodeFile.Subtitles.Select(s => s.Language)
            });
            SetMapping<EpisodeFile, NextEpisodeDto>(episodeFile => new NextEpisodeDto
            {
                Fid = episodeFile.Id,
                Title = $"{episodeFile.SeriesDetails.Name} - S{episodeFile.SeasonNumber:D2}E{episodeFile.EpisodeNumber:D2} - {episodeFile.EpisodeDetails.Name}"
            });
            SetMapping<SubtitleFile, SubtitleFileDto>(subtitleFile => new SubtitleFileDto
            {
                Id = subtitleFile.Id,
                Language = subtitleFile.Language
            });
            
            SetMapping<Genre, GenreDto>(genre => new GenreDto
            {
                Id = genre.Id,
                Name = genre.Name
            });
            SetMapping<ProductionCompany, ProductionCompanyDto>(productionCompany => new ProductionCompanyDto
            {
                Id = productionCompany.Id,
                Name = productionCompany.Name,
                LogoPath = productionCompany.LogoPath,
                LogoBlurHash = productionCompany.LogoBlurHash,
                OriginCountry = productionCompany.OriginCountry
            });
            SetMapping<ProductionCountry, ProductionCountryDto>(productionCountry => new ProductionCountryDto
            {
                Iso31661 = productionCountry.Iso3166_1,
                Name = productionCountry.Name
            });
            SetMapping<SpokenLanguage, SpokenLanguageDto>(spokenLanguage => new SpokenLanguageDto
            {
                Iso6391 = spokenLanguage.Iso639_1,
                Name = spokenLanguage.Name
            });
            SetMapping<MovieCollection, MovieCollectionDto>(movieCollection => new MovieCollectionDto
            {
                Id = movieCollection.Id,
                Name = movieCollection.Name,
                BackdropPath = movieCollection.BackdropPath,
                BackdropBlurHash = movieCollection.BackdropBlurHash,
                PosterPath = movieCollection.PosterPath,
                PosterBlurHash = movieCollection.PosterBlurHash,
            });
            SetMapping<Network, NetworkDto>(network => new NetworkDto
            {
                Name = network.Name,
                LogoPath = network.LogoPath,
                LogoBlurHash = network.LogoBlurHash,
                OriginCountry = network.OriginCountry
            });
            SetMapping<Creator, CreatorDto>(creator => new CreatorDto
            {
                Name = creator.Name
            });


            SetMapping<User, UserDto>(user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Admin = user.Admin,
                Email = user.Email,
                HasChangedPassword = user.HasChangedPassword,
                Libraries = user.LibraryAccessIds
            });
            SetMapping<Library, LibraryDto>(library => new LibraryDto
            {
                Id = library.Id,
                Name = library.Name,
                Language = library.Language,
                Kind = library.Kind.ToString().ToLowerInvariant()
            });
            SetMapping<Library, AdminLibraryDto>(library => new AdminLibraryDto
            {
                Id = library.Id,
                Name = library.Name,
                Language = library.Language,
                Kind = library.Kind.ToString().ToLowerInvariant(),
                Path = library.Path
            });
        }
    }
}