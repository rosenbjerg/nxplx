using System;
using System.Collections.Generic;

namespace NxPlx.Models.Dto.Models
{
    public class EpisodeFileDto
    {
        public Guid id { get; set; }

        public int seasonNumber { get; set; }

        public int episodeNumber { get; set; }
        public IEnumerable<SubtitleFileDto> subtitles { get; set; }
    }
}