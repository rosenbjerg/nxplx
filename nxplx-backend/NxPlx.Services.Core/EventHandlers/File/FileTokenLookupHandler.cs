using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Models.Events.File;

namespace NxPlx.Core.Services.EventHandlers.File
{
    public class FileTokenLookupHandler : IEventHandler<FileTokenLookupQuery, string?>
    {
        private const string StreamPrefix = "stream";
        private readonly IDistributedCache _distributedCache;

        public FileTokenLookupHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public Task<string?> Handle(FileTokenLookupQuery query, CancellationToken cancellationToken = default)
        {
            return _distributedCache.GetStringAsync($"{StreamPrefix}:{query.Token}", cancellationToken);
        }
    }
}