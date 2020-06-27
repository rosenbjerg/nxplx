using System.Collections.Generic;

namespace NxPlx.Application.Models
{
    public class InfoDto : IDto
    {
        public int Id { get; set; }
        public int Fid { get; set; }
        
        public string Title { get; set; } = null!;
        public string Poster { get; set; } = null!;
        public string Backdrop { get; set; } = null!;
        public IEnumerable<string> Subtitles { get; set; } = null!;
        public float Duration { get; set; }
    }
}