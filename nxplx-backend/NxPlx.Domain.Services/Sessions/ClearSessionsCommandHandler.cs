using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Domain.Events.Sessions;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.Sessions
{
    public class ClearSessionsCommandHandler : IDomainEventHandler<ClearSessionsCommand>
    {
        private const string SessionPrefix = "session";
        private readonly IDistributedCache _distributedCache;

        public ClearSessionsCommandHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task Handle(ClearSessionsCommand command, CancellationToken cancellationToken = default)
        {
            await _distributedCache.ClearList(SessionPrefix, command.UserId.ToString(), cancellationToken);
        }
    }
}