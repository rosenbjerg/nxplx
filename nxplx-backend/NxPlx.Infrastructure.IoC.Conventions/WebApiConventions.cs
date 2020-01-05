using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Infrastructure.Logging;
using NxPlx.Integrations.TMDb;
using NxPlx.Services.Caching;
using NxPlx.Services.Database;
using NxPlx.Services.Database.Wrapper;
using NxPlx.Services.Index;

namespace NxPlx.Infrastructure.IoC.Conventions
{
    public static class WebApiConventions
    {
        public static void Install()
        {
            var registration = new RegistrationContainer();
            registration.Register<ICachingService, RedisCachingService>();
            registration.Register<IBroadcaster, WebSocketBroadcaster>();
            registration.Register<ILoggingService, NLoggingService>();
            registration.Register<IDatabaseMapper, DatabaseMapper>();
            registration.Register<IDtoMapper, DtoMapper>();
            registration.Register<IDetailsApi, TMDbApi>(false);
            registration.Register<IIndexer, Indexer>(false);
            registration.Register<IReadNxplxContext, ReadNxplxContext>(false);
        }
    }
}