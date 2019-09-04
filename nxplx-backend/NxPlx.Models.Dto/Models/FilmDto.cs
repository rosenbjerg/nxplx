using System;
using System.Collections.Generic;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;

// ReSharper disable InconsistentNaming

namespace NxPlx.Models.Dto.Models
{
    public class FilmDto
    {
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