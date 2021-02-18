using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Models.Events.File
{
    public class FilePathLookupQuery : IDomainQuery<string?>
    {
        public FilePathLookupQuery(StreamKind streamKind, long id)
        {
            StreamKind = streamKind;
            Id = id;
        }

        public StreamKind StreamKind { get; }
        public long Id { get; }
    }
}