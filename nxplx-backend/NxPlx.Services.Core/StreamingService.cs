using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace NxPlx.Core.Services
{
    public class StreamingService
    {
        private const string StreamPrefix = "stream";
        private readonly IDistributedCache _distributedCache;

        public StreamingService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        
        public Task<string?> GetFilePath(string token, CancellationToken cancellationToken = default)
        {
            return _distributedCache.GetStringAsync($"{StreamPrefix}:{token}", cancellationToken);
        }

        public async Task<string> CreateToken(string filePath, CancellationToken cancellationToken = default)
        {
            var token = TokenGenerator.Generate();
            await _distributedCache.SetStringAsync($"{StreamPrefix}:{token}", filePath, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(35)
            }, cancellationToken);
            return token;
        }
    }
}