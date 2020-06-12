using System;
using System.Collections.Generic;

namespace NxPlx.Application.Models.Series
{
    public class SeasonDto : IDto
    {
        public DateTime? airDate { get; set; }
        public string name { get; set; } = null!;
        public string overview { get; set; } = null!;
        public string poster { get; set; } = null!;
        public int number { get; set; }
        public IEnumerable<EpisodeDto> episodes { get; set; } = null!;
    }
}