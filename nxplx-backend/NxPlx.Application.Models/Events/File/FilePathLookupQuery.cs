namespace NxPlx.Application.Models.Events.File
{
    public class FilePathLookupQuery : IQuery<string?>
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