using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.Sessions;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers.Sessions
{
    public class AllSessionsQueryHandler : IDomainEventHandler<SessionsQuery, SessionDto[]>
    {
        private const string SessionPrefix = "session";
        private readonly IDistributedCache _distributedCache;
        private readonly IDomainEventDispatcher _eventDispatcher;

        public AllSessionsQueryHandler(IDistributedCache distributedCache, IDomainEventDispatcher eventDispatcher)
        {
            _distributedCache = distributedCache;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<SessionDto[]> Handle(SessionsQuery @event, CancellationToken cancellationToken = default)
        {
            var sessions = await _distributedCache.GetObjectAsync<List<string>>($"{SessionPrefix}:{@event.UserId}", cancellationToken);
            if (sessions == null)
                return new SessionDto[0];

            var foundSessions = await Task.WhenAll(sessions.Select(async token => (token, session: await _eventDispatcher.Dispatch(new SessionQuery(token)))));
            return foundSessions.Where(s => s.session != null).Select(s => new SessionDto
            {
                Token = s.token,
                UserAgent = s.session!.UserAgent
            }).ToArray();
        }
    }

    public class SingleSessionQueryHandler : IDomainEventHandler<SessionQuery, Session?>
    {
        private const string SessionPrefix = "session";
        private readonly IDistributedCache _distributedCache;

        public SingleSessionQueryHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public Task<Session?> Handle(SessionQuery @event, CancellationToken cancellationToken = default)
        {
            return _distributedCache.GetObjectAsync<Session>($"{SessionPrefix}:{@event.Token}", cancellationToken);
        }
    }
}