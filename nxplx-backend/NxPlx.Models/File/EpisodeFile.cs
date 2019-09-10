using NxPlx.Services.Database.Models;

namespace NxPlx.Models.File
{
    public class EpisodeFile : MediaFileBase
    {
        public string Name { get; set; }

        public int SeasonNumber { get; set; }

        public int EpisodeNumber { get; set; }

        public virtual DbSeriesDetails SeriesDetails { get; set; }
        public int SeriesDetailsId { get; set; }

        public override string ToString()
        {
            return $"{Name} S{SeasonNumber}E{EpisodeNumber}";
        }
    }
}