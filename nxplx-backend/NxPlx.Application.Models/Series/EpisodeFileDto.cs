using System.Collections.Generic;

namespace NxPlx.Application.Models.Series
{
    public class EpisodeFileDto : IDto
    {
        public int id { get; set; }

        public int seasonNumber { get; set; }

        public int episodeNumber { get; set; }
        public IEnumerable<string> subtitles { get; set; } = null!;
    }
}