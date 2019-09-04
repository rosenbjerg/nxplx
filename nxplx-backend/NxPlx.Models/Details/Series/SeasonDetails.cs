using System;
using System.Collections.Generic;

namespace NxPlx.Models.Details.Series
{
    public class SeasonDetails : EntityBase
    {
        public DateTime? AirDate { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public int SeasonNumber { get; set; }
        public List<EpisodeDetails> Episodes { get; set; }
    }
}