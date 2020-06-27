using System;

namespace NxPlx.Application.Models
{
    public class ContinueWatchingDto : IDto
    {
        public int FileId { get; set; }
        public string Title { get; set; } = null!;
        public string? Poster { get; set; }
        public string Kind { get; set; } = null!;
        public DateTime Watched { get; set; }
        public double Progress { get; set; }
    }
}