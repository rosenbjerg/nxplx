using NxPlx.Models;

namespace NxPlx.Application.Models.Events.Series
{
    public class SubtitlePathQuery : IQuery<string?>
    {
        public SubtitlePathQuery(MediaFileType mediaType, int fileId, string language)
        {
            MediaType = mediaType;
            FileId = fileId;
            Language = language;
        }

        public MediaFileType MediaType { get; }
        public int FileId { get; }
        public string Language { get; }
    }
}