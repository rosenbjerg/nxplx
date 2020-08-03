using System.Collections.Generic;
using NxPlx.Application.Models.Film;

namespace NxPlx.Application.Models.Series
{
    public class SeriesDto : IDto
    {
        public int Id { get; set; }

        public string BackdropPath { get; set; } = null!;
        public string PosterPath { get; set; } = null!;

        public double VoteAverage { get; set; }

        public int VoteCount { get; set; }

        public string Name { get; set; } = null!;

        public IEnumerable<NetworkDto> Networks { get; set; } = null!;

        public IEnumerable<GenreDto> Genres { get; set; } = null!;

        public IEnumerable<CreatorDto> CreatedBy { get; set; } = null!;

        public IEnumerable<ProductionCompanyDto> ProductionCompanies { get; set; } = null!;

        public string Overview { get; set; } = null!;
        public IEnumerable<SeasonDto> Seasons { get; set; } = null!;
        public string BackdropBlurHash { get; set; } = null!;
        public string PosterBlurHash { get; set; } = null!;
    }
}