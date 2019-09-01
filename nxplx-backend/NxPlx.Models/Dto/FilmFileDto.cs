using System.Collections.Generic;
using System.Linq;
using NxPlx.Models.File;

namespace NxPlx.Models.Dto
{
    public class FilmFileDto
    {
        public FilmFileDto(FilmFile filmFile)
        {
            Title = filmFile.Title;
            Id = filmFile.Id;
            Subtitles = filmFile.Subtitles.Select(s => new SubtitleFileDto(s));
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public IEnumerable<SubtitleFileDto> Subtitles { get; set; }
    }
}