using System.Threading.Tasks;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Series
{
    public class SetWatchingProgressCommand : IDomainCommand<Task>
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