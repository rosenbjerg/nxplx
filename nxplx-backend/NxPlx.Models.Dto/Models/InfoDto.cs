using System.Collections.Generic;

namespace NxPlx.Models.Dto.Models
{
    public class InfoDto
    {
        public int id { get; set; }
        public int fid { get; set; }
        
        public string title { get; set; }
        public string poster { get; set; }
        public string backdrop { get; set; }
        public IEnumerable<string> subtitles { get; set; }
    }
}