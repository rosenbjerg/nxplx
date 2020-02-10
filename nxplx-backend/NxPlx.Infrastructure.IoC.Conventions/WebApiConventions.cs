using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Infrastructure.Logging;
using NxPlx.Integrations.TMDb;
using NxPlx.Services.Caching;
using NxPlx.Services.Database;
using NxPlx.Services.Database.Wrapper;
using NxPlx.Services.Index;
using Red;

namespace NxPlx.Infrastructure.IoC.Conventions
{
    public static class WebApiConventions
    {
        public static void Install()
        {
            var registration = new RegistrationContainer();
            var broadcaster = new WebSocketBroadcaster();
            registration.Register<IBroadcaster, WebSocketBroadcaster>(broadcaster);
            registration.Register<IBroadcaster<WebSocketDialog>, WebSocketBroadcaster>(broadcaster);
            registration.Register<ICachingService, RedisCachingService>();
            registration.Register<ILoggingService, NLoggingService>();
            registration.Register<IDatabaseMapper, DatabaseMapper>();
            registration.Register<IDtoMapper, DtoMapper>();
            registration.Register<IDetailsApi, TMDbApi>();
            registration.Register<IIndexer, Indexer>(false);
            registration.Register<IReadNxplxContext, ReadNxplxContext>(false);
        }
    }
}