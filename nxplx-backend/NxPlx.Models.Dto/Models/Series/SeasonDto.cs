using System;
using System.Collections.Generic;

namespace NxPlx.Models.Dto.Models.Series
{
    public class SeasonDto
    {
        public DateTime? airDate { get; set; }
        public string name { get; set; }
        public string overview { get; set; }
        public string poster { get; set; }
        public int number { get; set; }
        public IEnumerable<EpisodeDto> episodes { get; set; }
    }
}