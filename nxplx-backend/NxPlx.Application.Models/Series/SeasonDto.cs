using System;
using System.Collections.Generic;

namespace NxPlx.Application.Models.Series
{
    public class SeasonDto : IDto
    {
        public DateTime? AirDate { get; set; }
        public string Name { get; set; } = null!;
        public string Overview { get; set; } = null!;
        public string PosterPath { get; set; } = null!;
        public int Number { get; set; }
        public IEnumerable<EpisodeDto> Episodes { get; set; } = null!;
        public string PosterBlurHash { get; set; } = null!;
    }
}