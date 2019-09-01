using NxPlx.Models.Details.Series;

namespace NxPlx.Models.Database.Series
{
    public class CreatedBy
    {
        public int SeriesDetailsId { get; set; }
        public SeriesDetails SeriesDetails { get; set; }
        
        public int CreatorId { get; set; }
        public virtual Creator Creator { get; set; }
    }
}