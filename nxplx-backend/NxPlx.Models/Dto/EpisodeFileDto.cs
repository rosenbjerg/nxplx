using System.Collections.Generic;
using System.Linq;
using NxPlx.Models.File;

namespace NxPlx.Models.Dto
{
    public class EpisodeFileDto
    {
        public EpisodeFileDto(EpisodeFile episodeFile)
        {
            id = episodeFile.Id;
            seasonNumber = episodeFile.SeasonNumber;
            episodeNumber = episodeFile.EpisodeNumber;
            subtitles = episodeFile.Subtitles.Select(s => new SubtitleFileDto(s));
        }

        public int id { get; set; }

        public int seasonNumber { get; set; }

        public int episodeNumber { get; set; }
        public IEnumerable<SubtitleFileDto> subtitles { get; set; }
    }
}