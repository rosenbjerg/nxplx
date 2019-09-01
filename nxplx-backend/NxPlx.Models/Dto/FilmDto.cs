using System;
using System.Collections.Generic;
using System.Linq;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.File;

namespace NxPlx.Models.Dto
{
    public class FilmDto
    {
        public FilmDto(FilmDetails filmDetails, IList<FilmFile> film)
        {
            id = filmDetails.Id;
            backdropPath = filmDetails.BackdropPath;
            posterPath = filmDetails.PosterPath;
            voteAverage = filmDetails.VoteAverage;
            voteCount = filmDetails.VoteCount;
            title = filmDetails.Title;
            overview = filmDetails.Overview;
            budget = filmDetails.Budget;
            revenue = filmDetails.Revenue;
            releaseDate = filmDetails.ReleaseDate;
            runtime = filmDetails.Runtime;
            imdbId = filmDetails.ImdbId;
            tagline = filmDetails.Tagline;
            belongsToCollection = filmDetails.BelongsToCollection?.MovieCollection;
            genres = filmDetails.Genres?.Select(g => g.Genre);
            networks = filmDetails.SpokenLanguages?.Select(sl => sl.SpokenLanguage);
            productionCountry = filmDetails.ProductionCountries?.Select(pc => pc.ProductionCountry);
            productionCompanies = filmDetails.ProductionCompanies?.Select(pc => pc.ProductionCompany);
        }

        public int id { get; set; }

        public string backdropPath { get; set; }

        public string posterPath { get; set; }

        public double voteAverage { get; set; }

        public int voteCount { get; set; }

        public string title { get; set; }
        
        public DateTime? releaseDate { get; set; }
        public long budget { get; set; }
        public long revenue { get; set; }
        public int? runtime { get; set; }
        public string imdbId { get; set; }
        public string tagline { get; set; }
        public MovieCollection belongsToCollection { get; set; }

        public IEnumerable<SpokenLanguage> networks { get; set; }

        public IEnumerable<Genre> genres { get; set; }

        public IEnumerable<ProductionCountry> productionCountry { get; set; }

        public IEnumerable<ProductionCompany> productionCompanies { get; set; }

        public string overview { get; set; }
    }
}