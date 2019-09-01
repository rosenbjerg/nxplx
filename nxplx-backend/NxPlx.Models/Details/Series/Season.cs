namespace NxPlx.Models.Details.Series
{
    public class Season
    {
        public int SeriesDetailsId { get; set; }
        public SeriesDetails SeriesDetails { get; set; }
        
        public int SeasonDetailsId { get; set; }
        public virtual SeasonDetails SeasonDetails { get; set; }
    }
}