using NxPlx.Abstractions;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Infrastructure.Logging;
using NxPlx.Integrations.TMDb;
using NxPlx.Services.Caching;
using NxPlx.Services.Database;
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
            registration.Register<IDetailsApi, TMDbApi>(false);
            registration.Register<Indexer>(false);
            registration.Register<MediaContext>(false);
            registration.Register<UserContext>(false);

        }
    }
}