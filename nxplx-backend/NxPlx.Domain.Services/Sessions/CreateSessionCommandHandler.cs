using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Domain.Events.Sessions;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.Sessions
{
    public class CreateSessionCommandHandler : IDomainEventHandler<CreateSessionCommand>
    {
        private const string SessionPrefix = "session";
        private readonly IDistributedCache _distributedCache;

        public CreateSessionCommandHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task Handle(CreateSessionCommand command, CancellationToken cancellationToken = default)
        {
            await _distributedCache.AddToList(SessionPrefix, command.UserId.ToString(), command.Token, command.Session, command.Validity, cancellationToken);
        }
    }
}