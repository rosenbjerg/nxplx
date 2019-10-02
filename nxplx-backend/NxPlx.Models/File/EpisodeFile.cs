using NxPlx.Models.Database;

namespace NxPlx.Models.File
{
    public class EpisodeFile : MediaFileBase
    {
        public string Name { get; set; }

        public int SeasonNumber { get; set; }

        public int EpisodeNumber { get; set; }

        public virtual Library PartOfLibrary { get; set; }
        
        public int PartOfLibraryId { get; set; }
        
        public virtual DbSeriesDetails SeriesDetails { get; set; }
        public int? SeriesDetailsId { get; set; }

        public override string ToString()
        {
            return $"{Name} S{SeasonNumber}E{EpisodeNumber}";
        }
    }
}