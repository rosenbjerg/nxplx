using NxPlx.Models.File;

namespace NxPlx.Models.Dto
{
    public class SubtitleFileDto
    {
        public SubtitleFileDto(SubtitleFile subtitleFile)
        {
            id = subtitleFile.Id;
            language = subtitleFile.Language;
        }
        public int id { get; set; }

        public string language { get; set; }
    }
}