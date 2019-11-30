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
using FilmDetails = NxPlx.Models.Details.Film.FilmDetails;
using SeriesDetails = NxPlx.Models.Details.Series.SeriesDetails;

namespace NxPlx.Services.Database
{
    public class DatabaseMapper : MapperBase, IDatabaseMapper
    {
        public DatabaseMapper()
        {
            SetMapping<SeriesDetails, DbSeriesDetails>(tvDetails => new DbSeriesDetails
            {
                Id = tvDetails.Id,
                BackdropPath = tvDetails.BackdropPath,
                FirstAirDate = tvDetails.FirstAirDate,
                InProduction = tvDetails.InProduction,
                LastAirDate = tvDetails.LastAirDate,
                Name = tvDetails.Name,
                Overview = tvDetails.Overview,
                Popularity = tvDetails.Popularity,
                Type = tvDetails.Type,
                OriginalLanguage = tvDetails.OriginalLanguage,
                OriginalName = tvDetails.OriginalName,
                PosterPath = tvDetails.PosterPath,
                VoteAverage = tvDetails.VoteAverage,
                VoteCount = tvDetails.VoteCount,
                
                CreatedBy = tvDetails.CreatedBy?.Select(creator => new JoinEntity<DbSeriesDetails, Creator>()
                {
                    Entity1Id = tvDetails.Id,
                    Entity2Id = creator.Id
                }).ToList(),
                Genres = tvDetails.Genres?.Select(g => new JoinEntity<DbSeriesDetails, Genre>()
                {
                    Entity1Id = tvDetails.Id,
                    Entity2Id = g.Id
                }).ToList(),
                Networks = tvDetails.Networks?.Select(n => new JoinEntity<DbSeriesDetails, Network>()
                {
                    Entity1Id = tvDetails.Id,
                    Entity2Id = n.Id,
                }).ToList(),
                Seasons = tvDetails.Seasons,
                ProductionCompanies = tvDetails.ProductionCompanies?.Select(pb => new JoinEntity<DbSeriesDetails, ProductionCompany>()
                {
                    Entity1Id = tvDetails.Id,
                    Entity2Id = pb.Id,
                }).ToList(),
            });
            SetMapping<FilmDetails, DbFilmDetails>(movieDetails => new DbFilmDetails
            {
                Id = movieDetails.Id,
                BackdropPath = movieDetails.BackdropPath,
                Overview = movieDetails.Overview,
                Popularity = movieDetails.Popularity,
                OriginalLanguage = movieDetails.OriginalLanguage,
                PosterPath = movieDetails.PosterPath,
                VoteAverage = movieDetails.VoteAverage,
                VoteCount = movieDetails.VoteCount,
                Adult = movieDetails.Adult,
                Budget = movieDetails.Budget,
                Revenue = movieDetails.Revenue,
                Runtime = movieDetails.Runtime,
                Tagline = movieDetails.Tagline,
                Title = movieDetails.Title,
                ImdbId = movieDetails.ImdbId,
                OriginalTitle = movieDetails.OriginalTitle,
                ReleaseDate = movieDetails.ReleaseDate,
                BelongsInCollectionId = movieDetails.BelongsToCollection?.Id,
                
                Genres = movieDetails.Genres?.Select(genre => new JoinEntity<DbFilmDetails, Genre>
                {
                    Entity1Id = movieDetails.Id,
                    Entity2Id = genre.Id
                }).ToList(),
                SpokenLanguages = movieDetails.SpokenLanguages?.Select(spokenLanguage => new JoinEntity<DbFilmDetails, SpokenLanguage, string>
                {
                    Entity1Id = movieDetails.Id,
                    Entity2Id = spokenLanguage.Iso639_1
                }).ToList(),
                ProductionCountries = movieDetails.ProductionCountries?.Select(productionCountry => new JoinEntity<DbFilmDetails, ProductionCountry, string>
                {
                    Entity1Id = movieDetails.Id,
                    Entity2Id = productionCountry.Iso3166_1
                }).ToList(),
                ProductionCompanies = movieDetails.ProductionCompanies?.Select(productionCompany => new JoinEntity<DbFilmDetails, ProductionCompany>()
                {
                    Entity1Id = movieDetails.Id,
                    Entity2Id = productionCompany.Id
                }).ToList()
            });
            
            SetMapping<DbSeriesDetails, SeriesDto>(seriesDetails => new SeriesDto
            {
                id = seriesDetails.Id,
                backdrop = seriesDetails.BackdropPath,
                poster = seriesDetails.PosterPath,
                voteAverage = seriesDetails.VoteAverage,
                voteCount = seriesDetails.VoteCount,
                name = seriesDetails.Name,
                networks = MapMany<Network, NetworkDto>(seriesDetails.Networks.Select(n => n.Entity2)),
                genres = MapMany<Genre, GenreDto>(seriesDetails.Genres.Select(g => g.Entity2)),
                createdBy = MapMany<Creator, CreatorDto>(seriesDetails.CreatedBy.Select(cb => cb.Entity2)),
                productionCompanies = MapMany<ProductionCompany, ProductionCompanyDto>(seriesDetails.ProductionCompanies.Select(pc => pc.Entity2)),
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
                title = seriesDetails.Name
            });
            SetMapping<DbFilmDetails, OverviewElementDto>(filmDetails => new OverviewElementDto
            {
                id = filmDetails.Id,
                kind = "film",
                poster = filmDetails.PosterPath,
                title = filmDetails.Title
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
                genres = MapMany<Genre, GenreDto>(filmFilm.FilmDetails.Genres.Select(e => e.Entity2)).ToList(),
                overview = filmFilm.FilmDetails.Overview,
                popularity = filmFilm.FilmDetails.Popularity,
                revenue = filmFilm.FilmDetails.Revenue,
                runtime = filmFilm.FilmDetails.Runtime,
                tagline = filmFilm.FilmDetails.Tagline,
                imdbId = filmFilm.FilmDetails.ImdbId,
                originalLanguage = filmFilm.FilmDetails.OriginalLanguage,
                originalTitle = filmFilm.FilmDetails.OriginalTitle,
                posterPath = filmFilm.FilmDetails.PosterPath,
                productionCompanies = MapMany<ProductionCompany, ProductionCompanyDto>(filmFilm.FilmDetails.ProductionCompanies.Select(e => e.Entity2)).ToList(),
                productionCountries = MapMany<ProductionCountry, ProductionCountryDto>(filmFilm.FilmDetails.ProductionCountries.Select(e => e.Entity2)).ToList(),
                releaseDate = filmFilm.FilmDetails.ReleaseDate,
                spokenLanguages = MapMany<SpokenLanguage, SpokenLanguageDto>(filmFilm.FilmDetails.SpokenLanguages.Select(e => e.Entity2)).ToList(),
                voteAverage = filmFilm.FilmDetails.VoteAverage,
                voteCount = filmFilm.FilmDetails.VoteCount,
                belongsToCollection = Map<MovieCollection, MovieCollectionDto>(filmFilm.FilmDetails.BelongsInCollection),
            });
            
            SetMapping<FilmFile, InfoDto>(filmFilm => new InfoDto
            {
                id = filmFilm.FilmDetails.Id,
                fid = filmFilm.Id,
                backdrop = filmFilm.FilmDetails.BackdropPath,
                poster = filmFilm.FilmDetails.PosterPath,
                title = filmFilm.FilmDetails.Title,
                subtitles = filmFilm.Subtitles.Select(s => s.Language)
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
                backdrop = episodeFile.SeriesDetails.BackdropPath,
                poster = episodeFile.SeriesDetails.PosterPath,
                title = $"{episodeFile.SeriesDetails.Name} - S{episodeFile.SeasonNumber:D2}E{episodeFile.EpisodeNumber:D2}",
                subtitles = episodeFile.Subtitles.Select(s => s.Language)
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
                poster = movieCollection.PosterPath
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
                kind = library.Kind.ToString()
            });
            SetMapping<Library, AdminLibraryDto>(library => new AdminLibraryDto
            {
                id = library.Id,
                name = library.Name,
                language = library.Language,
                kind = library.Kind.ToString(),
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