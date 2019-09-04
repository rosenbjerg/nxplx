using System.Collections.Generic;
using System.Linq;
using NxPlx.Models.File;

namespace NxPlx.Models.Dto.Models
{
    public class FilmFileDto
    {
        public int id { get; set; }

        public string title { get; set; }

        public IEnumerable<SubtitleFileDto> subtitles { get; set; }
    }
}