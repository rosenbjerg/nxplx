using System;
using System.Collections.Generic;

namespace NxPlx.Domain.Models.Details.Series
{
    public class SeasonDetails : EntityBase, IPosterImageOwner
    {
        public DateTime? AirDate { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public string PosterBlurHash { get; set; }
        public int SeasonNumber { get; set; }
        public virtual List<EpisodeDetails> Episodes { get; set; }
    }
}