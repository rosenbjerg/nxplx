using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.File
{
    public class StreamUrlQuery : IDomainQuery<string>
    {
        public StreamKind StreamKind { get; }
        public long MediaFileId { get; }

        public StreamUrlQuery(StreamKind streamKind, long mediaFileId)
        {
            StreamKind = streamKind;
            MediaFileId = mediaFileId;
        }
    }
}