using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Configuration;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Logging;
using NxPlx.Integrations.TMDBApi;
using NxPlx.Services.Caching;
using NxPlx.Services.Database;
using NxPlx.Services.Index;
using NxPlx.WebApi.Routers;
using Red;

namespace NxPlx.WebApi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            var cfg = ConfigurationService.Current;

            {
                var registration = new RegistrationContainer();
                registration.Register<ICachingService, RedisCachingService>();
                registration.Register<ILogger, NLogger>();
                registration.Register<IDatabaseMapper, DatabaseMapper>();
                registration.Register<IDetailsMapper, TMDbMapper>(false);
                registration.Register<IDetailsApi, TmdbApi>(false);
                registration.Register<Indexer>(false);
            }
            
            var container = new ResolveContainer();
            var indexer = container.Resolve<Indexer>();
            var logger = container.Resolve<ILogger>();
            logger.Info("NxPlx.Infrastructure.WebApi starting...");
            
//            await indexer.IndexMovieLibrary(new []{""});
//            await indexer.IndexSeriesLibrary(new []{""});

           
            var server = new RedHttpServer(cfg.HttpPort);
            var databaseContextManager = new DatabaseContextManager();
            server.RespondWithExceptionDetails = true;
            server.ConfigureServices = databaseContextManager.Register;
            databaseContextManager.Initialize();
            
            EpisodeRoutes.Register(server.CreateRouter("/api/series"));
            FilmRoutes.Register(server.CreateRouter("/api/film"));
            OverviewRoutes.Register(server.CreateRouter("/api/overview"));
            ImageRoutes.Register(server.CreateRouter("/api/images"));
            
        
            await server.RunAsync();
        }
        
        
    }
    
}