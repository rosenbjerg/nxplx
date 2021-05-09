using NxPlx.Application.Models.Series;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Series
{
    public class NextEpisodeByFileIdQuery : IDomainQuery<NextEpisodeDto?>
    {
        public NextEpisodeByFileIdQuery(int fileId, string mode)
        {
            FileId = fileId;
            Mode = mode;
        }

        public int FileId { get; }
        public string Mode { get; }
    }
}