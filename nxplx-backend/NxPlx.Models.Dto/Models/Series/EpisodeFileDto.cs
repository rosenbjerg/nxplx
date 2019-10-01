using System.Collections.Generic;

namespace NxPlx.Models.Dto.Models.Series
{
    public class EpisodeFileDto
    {
        public int id { get; set; }

        public int seasonNumber { get; set; }

        public int episodeNumber { get; set; }
        public IEnumerable<string> subtitles { get; set; }
    }
}