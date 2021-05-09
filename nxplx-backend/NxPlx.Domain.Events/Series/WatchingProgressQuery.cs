using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Series
{
    public class WatchingProgressQuery : IDomainQuery<double>
    {
        public WatchingProgressQuery(MediaFileType mediaType, int fileId)
        {
            MediaType = mediaType;
            FileId = fileId;
        }

        public MediaFileType MediaType { get; }
        public int FileId { get; }
    }
}