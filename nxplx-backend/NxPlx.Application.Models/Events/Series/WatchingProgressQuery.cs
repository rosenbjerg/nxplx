using NxPlx.Models;

namespace NxPlx.Application.Models.Events.Series
{
    public class WatchingProgressQuery : IQuery<double>
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