using NxPlx.Models;

namespace NxPlx.Application.Models.Events.Series
{
    public class SetWatchingProgressCommand : ICommand
    {
        public SetWatchingProgressCommand(MediaFileType mediaType, int fileId, double progressValue)
        {
            MediaType = mediaType;
            FileId = fileId;
            ProgressValue = progressValue;
        }

        public MediaFileType MediaType { get; }
        public int FileId { get; }
        public double ProgressValue { get; }
    }
}