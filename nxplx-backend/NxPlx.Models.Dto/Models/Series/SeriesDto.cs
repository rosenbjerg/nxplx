using System.Collections.Generic;
using NxPlx.Models.Dto.Models.Film;

namespace NxPlx.Models.Dto.Models.Series
{
    public class SeriesDto
    {
        public int id { get; set; }

        public string backdrop { get; set; }
        public string poster { get; set; }

        public double voteAverage { get; set; }

        public int voteCount { get; set; }

        public string name { get; set; }

        public IEnumerable<NetworkDto> networks { get; set; }

        public IEnumerable<GenreDto> genres { get; set; }

        public IEnumerable<CreatorDto> createdBy { get; set; }

        public IEnumerable<ProductionCompanyDto> productionCompanies { get; set; }

        public string overview { get; set; }
        public IEnumerable<SeasonLiteDto> seasons { get; set; }
    }
}