namespace NxPlx.Models.Details.Series
{
    public class BroadcastOn
    {
        public int SeriesDetailsId { get; set; }
        public SeriesDetails SeriesDetails { get; set; }
        
        public int NetworkId { get; set; }
        public virtual Network Network { get; set; }
    }
}