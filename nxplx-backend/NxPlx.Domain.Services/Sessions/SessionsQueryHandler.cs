using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Domain.Events.Sessions;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.Sessions
{
    public class AllSessionsQueryHandler : IDomainEventHandler<SessionsQuery, SessionDto[]>
    {
        private const string SessionPrefix = "session";
        private readonly IDistributedCache _distributedCache;

        public AllSessionsQueryHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<SessionDto[]> Handle(SessionsQuery @event, CancellationToken cancellationToken = default)
        {
            var sessions = await _distributedCache.GetListElements<Session>(SessionPrefix, @event.UserId.ToString(), cancellationToken);
            return sessions.Select(s => new SessionDto
            {
                Token = s.Key,
                UserAgent = s.Element.UserAgent
            }).ToArray();
        }
    }
}