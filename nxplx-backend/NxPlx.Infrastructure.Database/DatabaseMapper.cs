using System.Linq;
using NxPlx.Application.Core;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using FilmDetails = NxPlx.Models.Details.Film.FilmDetails;
using SeriesDetails = NxPlx.Models.Details.Series.SeriesDetails;

namespace NxPlx.Infrastructure.Database
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
                
                CreatedBy = tvDetails.CreatedBy,
                Genres = tvDetails.Genres,
                Networks = tvDetails.Networks,
                Seasons = tvDetails.Seasons,
                ProductionCompanies = tvDetails.ProductionCompanies
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
                
                Genres = movieDetails.Genres,
                SpokenLanguages = movieDetails.SpokenLanguages,
                ProductionCountries = movieDetails.ProductionCountries,
                ProductionCompanies = movieDetails.ProductionCompanies.ToList()
            });
            
   
        }
    }
}