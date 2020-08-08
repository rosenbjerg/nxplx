using System.Linq;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
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
                
                CreatedBy = tvDetails.CreatedBy == null ? null : tvDetails.CreatedBy.Select(creator => new JoinEntity<DbSeriesDetails, Creator>()
                {
                    Entity1Id = tvDetails.Id,
                    Entity2Id = creator.Id
                }).ToList(),
                Genres = tvDetails.Genres == null ? null : tvDetails.Genres.Select(g => new JoinEntity<DbSeriesDetails, Genre>()
                {
                    Entity1Id = tvDetails.Id,
                    Entity2Id = g.Id
                }).ToList(),
                Networks = tvDetails.Networks == null ? null : tvDetails.Networks.Select(n => new JoinEntity<DbSeriesDetails, Network>()
                {
                    Entity1Id = tvDetails.Id,
                    Entity2Id = n.Id,
                }).ToList(),
                Seasons = tvDetails.Seasons,
                ProductionCompanies = tvDetails.ProductionCompanies == null ? null : tvDetails.ProductionCompanies.Select(pb => new JoinEntity<DbSeriesDetails, ProductionCompany>()
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
                BelongsInCollectionId = movieDetails.BelongsToCollection == null ? null : movieDetails.BelongsToCollection.Id as int?,
                
                Genres = movieDetails.Genres == null ? null : movieDetails.Genres.Select(genre => new JoinEntity<DbFilmDetails, Genre>
                {
                    Entity1Id = movieDetails.Id,
                    Entity2Id = genre.Id
                }).ToList(),
                SpokenLanguages = movieDetails.SpokenLanguages == null ? null : movieDetails.SpokenLanguages.Select(spokenLanguage => new JoinEntity<DbFilmDetails, SpokenLanguage, string>
                {
                    Entity1Id = movieDetails.Id,
                    Entity2Id = spokenLanguage.Iso639_1
                }).ToList(),
                ProductionCountries = movieDetails.ProductionCountries == null ? null : movieDetails.ProductionCountries.Select(productionCountry => new JoinEntity<DbFilmDetails, ProductionCountry, string>
                {
                    Entity1Id = movieDetails.Id,
                    Entity2Id = productionCountry.Iso3166_1
                }).ToList(),
                ProductionCompanies = movieDetails.ProductionCompanies == null ? null : movieDetails.ProductionCompanies.Select(productionCompany => new JoinEntity<DbFilmDetails, ProductionCompany>()
                {
                    Entity1Id = movieDetails.Id,
                    Entity2Id = productionCompany.Id
                }).ToList()
            });
            
   
        }
    }
}