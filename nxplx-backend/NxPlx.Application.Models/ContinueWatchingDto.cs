using System;

namespace NxPlx.Application.Models
{
    public class ContinueWatchingDto : IDto
    {
        public int fileId { get; set; }
        public string title { get; set; } = null!;
        public string? poster { get; set; }
        public string kind { get; set; } = null!;
        public DateTime watched { get; set; }
        public double progress { get; set; }
    }
}