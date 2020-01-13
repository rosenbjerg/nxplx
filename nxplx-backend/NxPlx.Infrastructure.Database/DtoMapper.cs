using System.Linq;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.Dto.Models.Film;
using NxPlx.Models.Dto.Models.Series;
using NxPlx.Models.File;

namespace NxPlx.Services.Database
{
    public class DtoMapper : MapperBase, IDtoMapper {
        public DtoMapper()
        {
            SetMapping<DbSeriesDetails, SeriesDto>(seriesDetails => new SeriesDto
            {
                id = seriesDetails.Id,
                backdrop = seriesDetails.BackdropPath,
                poster = seriesDetails.PosterPath,
                voteAverage = seriesDetails.VoteAverage,
                voteCount = seriesDetails.VoteCount,
                name = seriesDetails.Name,
                networks = Map<Network, NetworkDto>(seriesDetails.Networks.Select(n => n.Entity2)).ToList(),
                genres = Map<Genre, GenreDto>(seriesDetails.Genres.Select(g => g.Entity2)).ToList(),
                createdBy = Map<Creator, CreatorDto>(seriesDetails.CreatedBy.Select(cb => cb.Entity2)).ToList(),
                productionCompanies = Map<ProductionCompany, ProductionCompanyDto>(seriesDetails.ProductionCompanies.Select(pc => pc.Entity2)).ToList(),
                overview = seriesDetails.Overview
            });
            SetMapping<SeasonDetails, SeasonDto>(seasonDetails => new SeasonDto
            {
                name = seasonDetails.Name,
                number = seasonDetails.SeasonNumber,
                airDate = seasonDetails.AirDate,
                poster = seasonDetails.PosterPath,
                overview = seasonDetails.Overview,
            });
            SetMapping<DbSeriesDetails, OverviewElementDto>(seriesDetails => new OverviewElementDto
            {
                id = seriesDetails.Id,
                kind = "series",
                poster = seriesDetails.PosterPath,
                title = seriesDetails.Name,
                genres = seriesDetails.Genres.Select(g => g.Entity2Id).ToList(),
                year = seriesDetails.FirstAirDate?.Year ?? 9999
            });
            SetMapping<DbFilmDetails, OverviewElementDto>(filmDetails => new OverviewElementDto
            {
                id = filmDetails.Id,
                kind = "film",
                poster = filmDetails.PosterPath,
                title = filmDetails.Title,
                genres = filmDetails.Genres.Select(g => g.Entity2Id).ToList(),
                year = filmDetails.ReleaseDate?.Year ?? 9999
            });
            SetMapping<MovieCollection, OverviewElementDto>(movieCollection => new OverviewElementDto
            {
                id = movieCollection.Id,
                kind = "collection",
                poster = movieCollection.PosterPath,
                title = movieCollection.Name,
                genres = movieCollection.Movies.SelectMany(f => f.Genres).Select(g => g.Entity2Id).Distinct().ToList(),
                year = movieCollection.Movies.Min(f => f.ReleaseDate?.Year ?? 9999)
            });
            SetMapping<(WatchingProgress wp, EpisodeFile ef), ContinueWatchingDto>(pair => new ContinueWatchingDto
            {
                fileId = pair.ef.Id,
                kind = "series",
                poster = pair.ef.SeasonDetails.PosterPath ?? pair.ef.SeasonDetails.PosterPath,
                title = $"{pair.ef.SeriesDetails?.Name} - {pair.ef.GetNumber()} - {pair.ef.EpisodeDetails?.Name}",
                watched = pair.wp.LastWatched,
                progress = pair.wp.Time
            });
            SetMapping<(WatchingProgress wp, FilmFile ff), ContinueWatchingDto>(pair => new ContinueWatchingDto
            {
                fileId = pair.ff.Id,
                kind = "film",
                poster = pair.ff.FilmDetails.PosterPath,
                title = $"{pair.ff.FilmDetails?.Title}",
                watched = pair.wp.LastWatched,
                progress = pair.wp.Time
            });

            SetMapping<FilmFile, FilmDto>(filmFilm => new FilmDto
            {
                id = filmFilm.FilmDetails.Id,
                fid = filmFilm.Id,
                library = filmFilm.PartOfLibraryId,
                backdrop = filmFilm.FilmDetails.BackdropPath,
                poster = filmFilm.FilmDetails.PosterPath,
                title = filmFilm.FilmDetails.Title,
                budget = filmFilm.FilmDetails.Budget,
                genres = Map<Genre, GenreDto>(filmFilm.FilmDetails.Genres.Select(e => e.Entity2)).ToList(),
                overview = filmFilm.FilmDetails.Overview,
                popularity = filmFilm.FilmDetails.Popularity,
                revenue = filmFilm.FilmDetails.Revenue,
                runtime = filmFilm.FilmDetails.Runtime,
                tagline = filmFilm.FilmDetails.Tagline,
                imdbId = filmFilm.FilmDetails.ImdbId,
                originalLanguage = filmFilm.FilmDetails.OriginalLanguage,
                originalTitle = filmFilm.FilmDetails.OriginalTitle,
                posterPath = filmFilm.FilmDetails.PosterPath,
                productionCompanies = Map<ProductionCompany, ProductionCompanyDto>(filmFilm.FilmDetails.ProductionCompanies.Select(e => e.Entity2)).ToList(),
                productionCountries = Map<ProductionCountry, ProductionCountryDto>(filmFilm.FilmDetails.ProductionCountries.Select(e => e.Entity2)).ToList(),
                releaseDate = filmFilm.FilmDetails.ReleaseDate,
                spokenLanguages = Map<SpokenLanguage, SpokenLanguageDto>(filmFilm.FilmDetails.SpokenLanguages.Select(e => e.Entity2)).ToList(),
                voteAverage = filmFilm.FilmDetails.VoteAverage,
                voteCount = filmFilm.FilmDetails.VoteCount,
                belongsToCollectionId = filmFilm.FilmDetails.BelongsInCollectionId
            });
            
            SetMapping<FilmFile, InfoDto>(filmFile => new InfoDto
            {
                id = filmFile.FilmDetails.Id,
                fid = filmFile.Id,
                duration = filmFile.MediaDetails.Duration,
                backdrop = filmFile.FilmDetails.BackdropPath,
                poster = filmFile.FilmDetails.PosterPath,
                title = filmFile.FilmDetails.Title,
                subtitles = filmFile.Subtitles.Select(s => s.Language)
            });
            SetMapping<EpisodeFile, EpisodeFileDto>(episodeFile => new EpisodeFileDto
            {
                id = episodeFile.Id,
                episodeNumber = episodeFile.EpisodeNumber,
                seasonNumber = episodeFile.SeasonNumber,
                subtitles = episodeFile.Subtitles.Select(s => s.Language)
            });
            SetMapping<EpisodeFile, InfoDto>(episodeFile => new InfoDto
            {
                id = episodeFile.SeriesDetails.Id,
                fid = episodeFile.Id,
                duration = episodeFile.MediaDetails.Duration,
                backdrop = episodeFile.SeriesDetails.BackdropPath,
                poster = episodeFile.SeriesDetails.PosterPath,
                title = $"{episodeFile.SeriesDetails.Name} - S{episodeFile.SeasonNumber:D2}E{episodeFile.EpisodeNumber:D2}",
                subtitles = episodeFile.Subtitles.Select(s => s.Language)
            });
            SetMapping<EpisodeFile, NextEpisodeDto>(episodeFile => new NextEpisodeDto
            {
                fid = episodeFile.Id,
                poster = episodeFile.SeriesDetails.PosterPath,
                title = $"{episodeFile.SeriesDetails.Name} - S{episodeFile.SeasonNumber:D2}E{episodeFile.EpisodeNumber:D2}"
            });
            SetMapping<SubtitleFile, SubtitleFileDto>(subtitleFile => new SubtitleFileDto
            {
                id = subtitleFile.Id,
                language = subtitleFile.Language
            });
            
            SetMapping<Genre, GenreDto>(genre => new GenreDto
            {
                id = genre.Id,
                name = genre.Name
            });
            SetMapping<ProductionCompany, ProductionCompanyDto>(productionCompany => new ProductionCompanyDto
            {
                id = productionCompany.Id,
                name = productionCompany.Name,
                logo = productionCompany.LogoPath,
                originCountry = productionCompany.OriginCountry
            });
            SetMapping<ProductionCountry, ProductionCountryDto>(productionCountry => new ProductionCountryDto
            {
                iso3166_1 = productionCountry.Iso3166_1,
                name = productionCountry.Name
            });
            SetMapping<SpokenLanguage, SpokenLanguageDto>(spokenLanguage => new SpokenLanguageDto
            {
                iso639_1 = spokenLanguage.Iso639_1,
                name = spokenLanguage.Name
            });
            SetMapping<MovieCollection, MovieCollectionDto>(movieCollection => new MovieCollectionDto
            {
                id = movieCollection.Id,
                name = movieCollection.Name,
                backdrop = movieCollection.BackdropPath,
                poster = movieCollection.PosterPath,
            });
            SetMapping<Network, NetworkDto>(network => new NetworkDto
            {
                name = network.Name,
                logo = network.LogoPath,
                originCountry = network.OriginCountry
            });
            SetMapping<Creator, CreatorDto>(creator => new CreatorDto
            {
                name = creator.Name
            });


            SetMapping<User, UserDto>(user => new UserDto
            {
                id = user.Id,
                username = user.Username,
                isAdmin = user.Admin,
                email = user.Email,
                passwordChanged = user.HasChangedPassword,
                libraries = user.LibraryAccessIds
            });
            SetMapping<Library, LibraryDto>(library => new LibraryDto
            {
                id = library.Id,
                name = library.Name,
                language = library.Language,
                kind = library.Kind.ToString().ToLowerInvariant()
            });
            SetMapping<Library, AdminLibraryDto>(library => new AdminLibraryDto
            {
                id = library.Id,
                name = library.Name,
                language = library.Language,
                kind = library.Kind.ToString().ToLowerInvariant(),
                path = library.Path
            });
            
            
            SetMapping<UserSession, UserSessionDto>(userSession => new UserSessionDto
            {
                id = userSession.Id,
                userAgent = userSession.UserAgent,
                expiration = userSession.Expiration
            });
        }
    }
}