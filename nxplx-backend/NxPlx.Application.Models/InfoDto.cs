using System.Collections.Generic;

namespace NxPlx.Application.Models
{
    public class InfoDto : IDto
    {
        public int id { get; set; }
        public int fid { get; set; }
        
        public string title { get; set; } = null!;
        public string poster { get; set; } = null!;
        public string backdrop { get; set; } = null!;
        public IEnumerable<string> subtitles { get; set; } = null!;
        public float duration { get; set; }
    }
}