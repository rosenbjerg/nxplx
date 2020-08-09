using System.Linq;
using System.Threading.Tasks;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using StackExchange.Redis;

namespace NxPlx.ApplicationHost.Api
{
    public class RedisCacheClearer : ICacheClearer
    {
        private readonly ConnectionMultiplexer _connection;

        public RedisCacheClearer(ConnectionStrings connectionStrings)
        {
            _connection = ConnectionMultiplexer.Connect(connectionStrings.Redis);
        }
        
        
        public async Task Clear(string pattern)
        {
            if (!pattern.Contains("*")) pattern += "*";
            var database = _connection.GetDatabase();
            foreach (var endpoint in _connection.GetEndPoints())
            {
                var server = _connection.GetServer(endpoint);
                var keys = server.Keys(pattern: pattern).ToArray();
                await database.KeyDeleteAsync(keys);
            }
        }
    }
}