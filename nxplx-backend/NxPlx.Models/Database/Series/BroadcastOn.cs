using NxPlx.Models.Details.Series;

namespace NxPlx.Models.Database.Series
{
    public class BroadcastOn
    {
        public int SeriesDetailsId { get; set; }
        public SeriesDetails SeriesDetails { get; set; }
        
        public int NetworkId { get; set; }
        public virtual Network Network { get; set; }
    }
}