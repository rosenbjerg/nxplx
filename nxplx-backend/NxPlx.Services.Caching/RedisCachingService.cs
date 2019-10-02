using System;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Configuration;
using StackExchange.Redis;

namespace NxPlx.Services.Caching
{
    public class RedisCachingService : ICachingService
    {
        private readonly ConnectionMultiplexer _connection;

        public RedisCachingService()
        {
            var cfg = ConfigurationService.Current;
            _connection = ConnectionMultiplexer.Connect($"{cfg.RedisHost},password={cfg.RedisPassword},name=NxPlx");
        }

        public async Task<string?> GetAsync(string key)
        {
            var db = _connection.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public Task SetAsync(string key, string value, CacheKind kind)
        {
            var db = _connection.GetDatabase();
            return db.StringSetAsync(key, value, TimeSpan.FromMinutes((int) kind));
        }

        public Task RemoveAsync(string keyOrPattern)
        {
            var db = _connection.GetDatabase();
            return keyOrPattern.Contains("*") 
                ? RemoveByPattern(db, keyOrPattern) 
                : db.KeyDeleteAsync(keyOrPattern);
        }

        private async Task RemoveByPattern(IDatabase db, string pattern)
        {
            foreach (var ep in _connection.GetEndPoints())
            {
                var server = _connection.GetServer(ep);
                var keys = server.Keys(pattern: pattern).ToArray();
                await db.KeyDeleteAsync(keys);
            }
        }
    }
}