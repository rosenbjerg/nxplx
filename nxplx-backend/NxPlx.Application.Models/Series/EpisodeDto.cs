using System;

namespace NxPlx.Application.Models.Series
{
    public class EpisodeDto : IDto
    {
        public DateTime? airDate { get; set; }
        public int number { get; set; }
        public string name { get; set; } = null!;
        public string still { get; set; } = null!;
        public int fileId { get; set; }
    }
}