using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Domain.Events.Sessions;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.Sessions
{
    public class RemoveSessionCommandHandler : IDomainEventHandler<RemoveSessionCommand>
    {
        private const string SessionPrefix = "session";
        private readonly IDistributedCache _distributedCache;

        public RemoveSessionCommandHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task Handle(RemoveSessionCommand command, CancellationToken cancellationToken = default)
        {
            var sessions = await _distributedCache.GetObjectAsync<List<string>>($"{SessionPrefix}:{command.UserId}", cancellationToken);
            await _distributedCache.RemoveAsync($"{SessionPrefix}:{command.Token}", cancellationToken);
            
            if (sessions != null && sessions.Remove(command.Token))
                await _distributedCache.SetObjectAsync($"{SessionPrefix}:{command.UserId}", sessions, cancellationToken: CancellationToken.None);
        }
    }
}