using System;

namespace NxPlx.Models.Dto.Models.Series
{
    public class EpisodeDto
    {
        public DateTime? airDate { get; set; }
        public int episodeNumber { get; set; }
        public string name { get; set; }
        public string overview { get; set; }
        public int seasonNumber { get; set; }
        public string still { get; set; }
        public float voteAverage { get; set; }
        public int voteCount { get; set; }
    }
}