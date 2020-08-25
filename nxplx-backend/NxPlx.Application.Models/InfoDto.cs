using System.Collections.Generic;

namespace NxPlx.Application.Models
{
    public class InfoDto : IDto
    {
        public int Id { get; set; }
        public string FileToken { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string PosterPath { get; set; } = null!;
        public string BackdropPath { get; set; } = null!;
        public IEnumerable<string> Subtitles { get; set; } = null!;
        public float Duration { get; set; }
        public string BackdropBlurHash { get; set; } = null!;
        public string PosterBlurHash { get; set; } = null!;
    }
}