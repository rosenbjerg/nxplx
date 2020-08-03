using System;

namespace NxPlx.Application.Models
{
    public class ContinueWatchingDto : IDto
    {
        public int FileId { get; set; }
        public string Title { get; set; } = null!;
        public string? PosterPath { get; set; }
        public string Kind { get; set; } = null!;
        public DateTime Watched { get; set; }
        public double Progress { get; set; }
        public string PosterBlurHash { get; set; } = null!;
    }
}