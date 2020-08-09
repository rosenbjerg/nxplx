using System.Collections.Generic;

namespace NxPlx.Application.Models.Series
{
    public class EpisodeFileDto : IDto
    {
        public int Id { get; set; }

        public int SeasonNumber { get; set; }

        public int EpisodeNumber { get; set; }
        public IEnumerable<string> Subtitles { get; set; } = null!;
    }
}