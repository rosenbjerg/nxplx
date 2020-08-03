using System.Collections.Generic;

namespace NxPlx.Application.Models
{
    public class OverviewElementDto : IDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string PosterPath { get; set; } = null!;
        public string Kind { get; set; } = null!;
        public int Year { get; set; }
        public List<int> Genres { get; set; } = null!;
        public string PosterBlurHash { get; set; } = null!;
    }
}