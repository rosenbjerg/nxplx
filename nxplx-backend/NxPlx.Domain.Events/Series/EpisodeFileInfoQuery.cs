using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Series
{
    public class EpisodeFileInfoQuery : IDomainQuery<EpisodeInfoDto?>
    {
        public EpisodeFileInfoQuery(int id)
        {
            Id = id;
        }
        
        public int Id { get; }
    }
}