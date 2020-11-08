using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Models.Events;

namespace NxPlx.Core.Services.EventHandlers
{
    public class FileTokenRequestHandler : IEventHandler<FileTokenRequestEvent, string>
    {
        private const string StreamPrefix = "stream";
        private readonly IDistributedCache _distributedCache;

        public FileTokenRequestHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        
        public async Task<string> Handle(FileTokenRequestEvent @event, CancellationToken cancellationToken = default)
        {
            var token = TokenGenerator.Generate();
            await _distributedCache.SetStringAsync($"{StreamPrefix}:{token}", @event.FilePath, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(35)
            }, cancellationToken);
            return token;
        }
    }
}