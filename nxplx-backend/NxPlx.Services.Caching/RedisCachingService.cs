using System.Net;
using Microsoft.Extensions.Caching.Redis;
using NxPlx.Configuration;
using StackExchange.Redis;

namespace NxPlx.Services.Caching
{
    public class RedisCachingService : CachingServiceBase<RedisCache>
    {
        public RedisCachingService() : base(new RedisCache(new RedisCacheOptions
        {
            Configuration = ConfigurationService.Current.RedisConfiguration,
            InstanceName = ConfigurationService.Current.RedisInstance,
            ConfigurationOptions = new ConfigurationOptions
            {
                ClientName = "NxPlx Caching Service",
                EndPoints = { ConfigurationService.Current.RedisConfiguration },
                Password = ConfigurationService.Current.RedisPassword
            }
        }))
        {
            
        }
    }
}