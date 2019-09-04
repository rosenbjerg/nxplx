using System.Collections.Generic;
using System.Linq;
using NxPlx.Models.File;

namespace NxPlx.Models.Dto.Models
{
    public class EpisodeFileDto
    {
        public int id { get; set; }

        public int seasonNumber { get; set; }

        public int episodeNumber { get; set; }
        public IEnumerable<SubtitleFileDto> subtitles { get; set; }
    }
}