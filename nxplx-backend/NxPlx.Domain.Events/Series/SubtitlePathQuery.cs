using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Series
{
    public class SubtitlePathQuery : IDomainQuery<string?>
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