using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Series
{
    public class SubtitleLanguagePreferenceQuery : IDomainQuery<string>
    {
        public SubtitleLanguagePreferenceQuery(MediaFileType mediaType, int fileId)
        {
            MediaType = mediaType;
            FileId = fileId;
        }
        
        public MediaFileType MediaType { get; }
        public int FileId { get; }
    }
}