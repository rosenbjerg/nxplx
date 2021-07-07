using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Domain.Events.Sessions;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.Sessions
{
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