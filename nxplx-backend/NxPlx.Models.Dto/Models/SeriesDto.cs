using System.Collections.Generic;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Series;

namespace NxPlx.Models.Dto.Models
{
    public class SeriesDto
    {
        public int id { get; set; }

        public string backdropPath { get; set; }
        public string posterPath { get; set; }

        public double voteAverage { get; set; }

        public int voteCount { get; set; }

        public string name { get; set; }

        public IEnumerable<Network> networks { get; set; }

        public IEnumerable<Genre> genres { get; set; }

        public IEnumerable<Creator> createdBy { get; set; }

        public IEnumerable<ProductionCompany> productionCompanies { get; set; }

        public string overview { get; set; }
        public IEnumerable<SeasonDetails> seasons { get; set; }
    }
}