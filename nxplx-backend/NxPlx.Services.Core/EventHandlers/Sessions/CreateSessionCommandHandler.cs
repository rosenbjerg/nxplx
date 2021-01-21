using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Sessions;

namespace NxPlx.Core.Services.EventHandlers.Sessions
{
    public class CreateSessionCommandHandler : IEventHandler<CreateSessionCommand>
    {
        private const string SessionPrefix = "session";
        private readonly IDistributedCache _distributedCache;

        public CreateSessionCommandHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task Handle(CreateSessionCommand command, CancellationToken cancellationToken = default)
        {
            var sessions = await _distributedCache.GetObjectAsync<List<string>>($"{SessionPrefix}:{command.UserId}", cancellationToken);
            sessions ??= new List<string>();
            await _distributedCache.SetObjectAsync($"{SessionPrefix}:{command.Token}", command.Session, new DistributedCacheEntryOptions
            {
                SlidingExpiration = command.Validity
            }, cancellationToken);
            
            sessions.Add(command.Token);
            await _distributedCache.SetObjectAsync($"{SessionPrefix}:{command.UserId}", sessions, cancellationToken: CancellationToken.None);
        }
    }
}