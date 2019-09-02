using System;
using System.Collections.Generic;

namespace NxPlx.Models.Details.Series
{
    public class SeriesDetails : DetailsEntityBase
    {
        public virtual List<Creator> CreatedBy { get; set; }
        public DateTime? FirstAirDate { get; set; }
        public bool InProduction { get; set; }
        public DateTime? LastAirDate { get; set; }
        public string Name { get; set; }
        public virtual List<Network> Networks { get; set; }
        public string OriginalName { get; set; }
        public virtual List<SeasonDetails> Seasons { get; set; }
        public string Type { get; set; }
    }
}