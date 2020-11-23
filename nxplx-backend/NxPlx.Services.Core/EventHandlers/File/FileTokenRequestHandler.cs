using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Models.Events.File;

namespace NxPlx.Core.Services.EventHandlers.File
{
    public class FileTokenRequestHandler : IEventHandler<RequestFileTokenCommand, string>
    {
        private const string StreamPrefix = "stream";
        private readonly IDistributedCache _distributedCache;

        public FileTokenRequestHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        
        public async Task<string> Handle(RequestFileTokenCommand command, CancellationToken cancellationToken = default)
        {
            var token = TokenGenerator.Generate();
            await _distributedCache.SetStringAsync($"{StreamPrefix}:{token}", command.FilePath, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(35)
            }, cancellationToken);
            return token;
        }
    }
}