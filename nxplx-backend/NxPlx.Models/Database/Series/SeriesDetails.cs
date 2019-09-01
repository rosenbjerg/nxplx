using System;
using System.Collections.Generic;

namespace NxPlx.Models.Database.Series
{
    public class SeriesDetails : DetailsEntityBase
    {
        public virtual List<CreatedBy> CreatedBy { get; set; }
        public DateTime? FirstAirDate { get; set; }
        public bool InProduction { get; set; }
        public DateTime? LastAirDate { get; set; }
        public string Name { get; set; }
        public virtual List<BroadcastOn> Networks { get; set; }
        public string OriginalName { get; set; }
        public virtual List<Season> Seasons { get; set; }
        public string Type { get; set; }
    }
}