using System;

namespace NxPlx.Application.Models.Series
{
    public class EpisodeDto : IDto
    {
        public DateTime? AirDate { get; set; }
        public int Number { get; set; }
        public string Name { get; set; } = null!;
        public string StillPath { get; set; } = null!;
        public int FileId { get; set; }
        public string StillBlurHash { get; set; } = null!;
    }
}