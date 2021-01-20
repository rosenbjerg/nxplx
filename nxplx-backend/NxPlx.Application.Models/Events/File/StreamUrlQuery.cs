namespace NxPlx.Application.Models.Events.File
{
    public class StreamUrlQuery : ICommand<string>
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