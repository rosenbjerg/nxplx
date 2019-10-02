using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using NxPlx.Abstractions;
using NxPlx.Configuration;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Logging;
using NxPlx.Infrastructure.Session;
using NxPlx.Integrations.TMDBApi;
using NxPlx.Models;
using NxPlx.Services.Caching;
using NxPlx.Services.Database;
using NxPlx.Services.Index;
using NxPlx.WebApi.Routers;
using Red;
using Red.CookieSessions;
using Red.CookieSessions.EFCore;

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
                registration.Register<ILoggingService, NLoggingService>();
                registration.Register<IDatabaseMapper, DatabaseMapper>();
                registration.Register<IDetailsMapper, TMDbMapper>(false);
                registration.Register<IDetailsApi, TmdbApi>(false);
                registration.Register<Indexer>(false);
                registration.Register<MediaContext>(false);
                registration.Register<UserContext>(false);

                var broadcaster = new WebSocketBroadcaster();
                registration.Register<IBroadcaster, WebSocketBroadcaster>(broadcaster);
                registration.Register<IBroadcaster<string, WebSocketDialog>, WebSocketBroadcaster>(broadcaster);
            }
            
            var container = new ResolveContainer();
            var logger = container.Resolve<ILoggingService>();
            var cacher = container.Resolve<ICachingService>();
            logger.Info("NxPlx.Infrastructure.WebApi starting...");

            await using var ctx = container.Resolve<MediaContext>();
            ctx.Database.EnsureCreated();
            if (!ctx.Libraries.Any())
            {
                var libraries = new List<Library>
                {
                    new Library
                    {
                        Name = "Movies",
                        Kind = LibraryKind.Film,
                        Language = "en-US",
                        Path = "\\\\raspberrypi.local\\nas1\\Media\\Film"
                    },
                    new Library
                    {
                        Name = "Series",
                        Kind = LibraryKind.Series,
                        Language = "en-US",
                        Path = "\\\\raspberrypi.local\\nas1\\Media\\Serier"
                    },
                    new Library
                    {
                        Name = "Disney",
                        Kind = LibraryKind.Film,
                        Language = "en-US",
                        Path = "\\\\raspberrypi.local\\nas1\\Media\\Disney"
                    },
                    new Library
                    {
                        Name = "Danske Film",
                        Kind = LibraryKind.Film,
                        Language = "da-DK",
                        Path = "\\\\raspberrypi.local\\nas1\\Media\\Danske film"
                    },
                    new Library
                    {
                        Name = "Danske Serier",
                        Kind = LibraryKind.Series,
                        Language = "da-DK",
                        Path = "\\\\raspberrypi.local\\nas1\\Media\\Danske serier"
                    },
                    new Library
                    {
                        Name = "Danske Disney",
                        Kind = LibraryKind.Film,
                        Language = "da-DK",
                        Path = "\\\\raspberrypi.local\\nas1\\Media\\Dansk Disney"
                    }
                };
                await ctx.AddRangeAsync(libraries);
                await ctx.SaveChangesAsync();
            }
            
            var server = new RedHttpServer(cfg.HttpPort);
            
            server.Use(new CookieSessions<UserSession>(TimeSpan.FromDays(14))
            {
                Secure = false,
                Path = "/",
                Store = new EntityFrameworkSessionStore<UserSession>(container.Resolve<UserContext>)
            });
            
            var databaseContextManager = new DatabaseContextManager();
            server.RespondWithExceptionDetails = true;
            server.ConfigureServices = databaseContextManager.Register;
            await databaseContextManager.Initialize();
            
            IndexingRoutes.Register(server.CreateRouter("/api/index"));
            UserRoutes.Register(server.CreateRouter("/api/user"));
            OverviewRoutes.Register(server.CreateRouter("/api/overview"));
            EpisodeRoutes.Register(server.CreateRouter("/api/series"));
            FilmRoutes.Register(server.CreateRouter("/api/film"));
            
            BroadcastRoutes.Register(server.CreateRouter("/api/broadcasts"));
            SubtitleRoutes.Register(server.CreateRouter("/api/subtitle"));
            ProgressRoutes.Register(server.CreateRouter("/api/progress"));
            ImageRoutes.Register(server.CreateRouter("/api/images"));
            
//            await indexer.IndexAllLibraries();
        
            await server.RunAsync();
        }
    }
}