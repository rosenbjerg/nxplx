using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Models.Events;

namespace NxPlx.Core.Services.EventHandlers
{
    public class FileTokenLookupHandler : IEventHandler<FileTokenLookupEvent, string?>
    {
        private const string StreamPrefix = "stream";
        private readonly IDistributedCache _distributedCache;

        public FileTokenLookupHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public Task<string?> Handle(FileTokenLookupEvent @event, CancellationToken cancellationToken = default)
        {
            return _distributedCache.GetStringAsync($"{StreamPrefix}:{@event.Token}", cancellationToken);
        }
    }
}