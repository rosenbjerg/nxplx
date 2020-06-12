using System.Collections.Generic;
using NxPlx.Application.Models.Film;

namespace NxPlx.Application.Models.Series
{
    public class SeriesDto : IDto
    {
        public int id { get; set; }

        public string backdrop { get; set; } = null!;
        public string poster { get; set; } = null!;

        public double voteAverage { get; set; }

        public int voteCount { get; set; }

        public string name { get; set; } = null!;

        public IEnumerable<NetworkDto> networks { get; set; } = null!;

        public IEnumerable<GenreDto> genres { get; set; } = null!;

        public IEnumerable<CreatorDto> createdBy { get; set; } = null!;

        public IEnumerable<ProductionCompanyDto> productionCompanies { get; set; } = null!;

        public string overview { get; set; } = null!;
        public IEnumerable<SeasonDto> seasons { get; set; } = null!;
    }
}