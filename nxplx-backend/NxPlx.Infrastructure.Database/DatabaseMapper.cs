using System.Linq;
using NxPlx.Abstractions;
using NxPlx.Models.Database;
using NxPlx.Models.Database.Film;
using NxPlx.Models.Database.Series;
using FilmDetails = NxPlx.Models.Details.Film.FilmDetails;
using SeriesDetails = NxPlx.Models.Details.Series.SeriesDetails;

namespace NxPlx.Services.Database
{
    public class DatabaseMapper : MapperBase, IDatabaseMapper
    {
        public DatabaseMapper()
        {
            
            SetMapping<SeriesDetails, NxPlx.Models.Database.Series.SeriesDetails>(tvDetails => new NxPlx.Models.Database.Series.SeriesDetails
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
            
                CreatedBy = tvDetails.CreatedBy?.Select(creator => new CreatedBy
                {
                    CreatorId = creator.Id,
                    SeriesDetailsId = tvDetails.Id
                }).ToList(),
                Genres = tvDetails.Genres?.Select(g => new InGenre
                {
                    GenreId = g.Id,
                    DetailsEntityId = tvDetails.Id
                }).ToList(),
                Networks = tvDetails.Networks?.Select(n => new BroadcastOn()
                {
                    NetworkId = n.Id,
                    SeriesDetailsId = tvDetails.Id
                }).ToList(),
                Seasons = tvDetails.Seasons?.Select(s => new Season
                {
                    SeasonDetailsId = s.Id,
                    SeriesDetailsId = tvDetails.Id
                }).ToList(),
                ProductionCompanies = tvDetails.ProductionCompanies?.Select(pb => new ProducedBy()
                {
                    ProductionCompanyId = pb.Id,
                    DetailsEntityId = tvDetails.Id
                }).ToList(),
            });
            
            SetMapping<FilmDetails, NxPlx.Models.Database.Film.FilmDetails>(movieDetails => new NxPlx.Models.Database.Film.FilmDetails
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
                
                
                BelongsToCollection = movieDetails.BelongsToCollection == null ? null : new BelongsInCollection
                {
                    MovieCollectionId = movieDetails.BelongsToCollection.Id,
                    FilmDetailsId = movieDetails.Id
                },
                Genres = movieDetails.Genres?.Select(genre => new InGenre
                {
                    GenreId = genre.Id,
                    DetailsEntityId = movieDetails.Id
                }).ToList(),
                SpokenLanguages = movieDetails.SpokenLanguages?.Select(spokenLanguage => new LanguageSpoken
                {
                    SpokenLanguageId = spokenLanguage.Iso639_1,
                    FilmDetailsId = movieDetails.Id
                }).ToList(),
                ProductionCountries = movieDetails.ProductionCountries?.Select(productionCountry => new ProducedIn
                {
                    ProductionCountryId = productionCountry.Iso3166_1,
                    FilmDetailsId = movieDetails.Id
                }).ToList(),
                ProductionCompanies = movieDetails.ProductionCompanies?.Select(productionCompany => new ProducedBy()
                {
                    ProductionCompanyId = productionCompany.Id,
                    DetailsEntityId = movieDetails.Id
                }).ToList()
            });
            
        }
    }
}