using System.Linq;
using NxPlx.Abstractions;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.Dto;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.File;
using NxPlx.Services.Database.Models;
using FilmDetails = NxPlx.Models.Details.Film.FilmDetails;
using SeriesDetails = NxPlx.Models.Details.Series.SeriesDetails;

namespace NxPlx.Services.Database
{
    public class DatabaseMapper : MapperBase, IDatabaseMapper
    {
        public DatabaseMapper(IDtoMapper dtoMapper)
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
                Seasons = tvDetails.Seasons?.Select(s => new JoinEntity<DbSeriesDetails, SeasonDetails>
                {
                    Entity1Id = tvDetails.Id,
                    Entity2Id = s.Id,
                }).ToList(),
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
                seasons = seriesDetails.Seasons.Select(s => s.Entity2)
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
                
                
                BelongsToCollection = movieDetails.BelongsToCollection == null ? null : new JoinEntity<DbFilmDetails, MovieCollection>
                {
                    Entity1Id = movieDetails.Id,
                    Entity2Id = movieDetails.BelongsToCollection.Id
                },
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
                belongsToCollection = filmDetails.BelongsToCollection?.Entity2,
                genres = filmDetails.Genres?.Select(g => g.Entity2),
                networks = filmDetails.SpokenLanguages?.Select(sl => sl.Entity2),
                productionCountry = filmDetails.ProductionCountries?.Select(pc => pc.Entity2),
                productionCompanies = filmDetails.ProductionCompanies?.Select(pc => pc.Entity2)
            });

        }
    }
}