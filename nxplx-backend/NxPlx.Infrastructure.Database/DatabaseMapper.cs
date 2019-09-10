using System.Linq;
using NxPlx.Abstractions;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.File;
using NxPlx.Services.Database.Models;
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
            
            SetMapping<DbSeriesDetails, SeriesDto>(seriesDetails => new SeriesDto
            {
                id = seriesDetails.Id,
                backdropPath = seriesDetails.BackdropPath,
                posterPath = seriesDetails.PosterPath,
                voteAverage = seriesDetails.VoteAverage,
                voteCount = seriesDetails.VoteCount,
                name = seriesDetails.Name,
                networks = seriesDetails.Networks.Select(n => n.Entity2),
                genres = seriesDetails.Genres.Select(g => g.Entity2),
                createdBy = seriesDetails.CreatedBy.Select(cb => cb.Entity2),
                productionCompanies = seriesDetails.ProductionCompanies.Select(pc => pc.Entity2),
                overview = seriesDetails.Overview,
                seasons = seriesDetails.Seasons
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
                BelongsInCollection = movieDetails.BelongsToCollection,
                
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
            
            SetMapping<DbFilmDetails, FilmDto>(filmDetails => new FilmDto
            {
                id = filmDetails.Id,
                backdropPath = filmDetails.BackdropPath,
                posterPath = filmDetails.PosterPath,
                voteAverage = filmDetails.VoteAverage,
                voteCount = filmDetails.VoteCount,
                title = filmDetails.Title,
                overview = filmDetails.Overview,
                budget = filmDetails.Budget,
                revenue = filmDetails.Revenue,
                releaseDate = filmDetails.ReleaseDate,
                runtime = filmDetails.Runtime,
                imdbId = filmDetails.ImdbId,
                tagline = filmDetails.Tagline,
                belongsToCollection = filmDetails.BelongsInCollection,
                genres = filmDetails.Genres?.Select(g => g.Entity2),
                networks = filmDetails.SpokenLanguages?.Select(sl => sl.Entity2),
                productionCountry = filmDetails.ProductionCountries?.Select(pc => pc.Entity2),
                productionCompanies = filmDetails.ProductionCompanies?.Select(pc => pc.Entity2)
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
            
            SetMapping<FilmFile, FilmInfoDto>(filmFilm => new FilmInfoDto
            {
                id = filmFilm.FilmDetails.Id,
                fid = filmFilm.Id,
                backdrop = filmFilm.FilmDetails.BackdropPath,
                poster = filmFilm.FilmDetails.PosterPath,
                subtitles = MapMany<SubtitleFile, SubtitleFileDto>(filmFilm.Subtitles),
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
            
            SetMapping<Genre, GenreDto>(genre => new GenreDto
            {
                id = genre.Id,
                name = genre.Name
            });
            SetMapping<ProductionCompany, ProductionCompanyDto>(productionCompany => new ProductionCompanyDto
            {
                id = productionCompany.Id,
                name = productionCompany.Name,
                logoPath = productionCompany.LogoPath,
                originCountry = productionCompany.OriginCountry
            });
            SetMapping<ProductionCountry, ProductionCountryDto>(productionCountry => new ProductionCountryDto
            {
                iso3166_1 = productionCountry.Iso3166_1,
                Name = productionCountry.Name
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
                backdropPath = movieCollection.BackdropPath,
                posterPath = movieCollection.PosterPath
            });
            SetMapping<SubtitleFile, SubtitleFileDto>(subtitleFile => new SubtitleFileDto
            {
                id = subtitleFile.Id,
                language = subtitleFile.Language
            });
        }
    }
}