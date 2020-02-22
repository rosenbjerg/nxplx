using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Configuration;
using NxPlx.Core.Services;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.IoC.Conventions;
using NxPlx.Infrastructure.Session;
using NxPlx.Infrastructure.WebApi.Routes;
using NxPlx.Models;
using NxPlx.Services.Database;
using Red;
using Red.CookieSessions;
using Red.CookieSessions.EFCore;
using Utils = NxPlx.Infrastructure.WebApi.Routes.Utils;

namespace NxPlx.WebApi
{
    class Program
    {
        private const int StaticFileCacheMaxAge = 60 * 60 * 24 * 2;
        static async Task Main(string[] args)
        {
            WebApiConventions.Install();

            var cfg = ConfigurationService.Current;
            var container = ResolveContainer.Default;
            var logger = container.Resolve<ILoggingService>();
            logger.Info("NxPlx.Infrastructure.WebApi starting...");

            if (!cfg.Production)
            {
                logger.Trace("Running in DEBUG mode");
            }

            var databaseContextManager = new DatabaseContextManager();
            var server = new RedHttpServer(cfg.HttpPort, "public")
            {
                RespondWithExceptionDetails = !cfg.Production,
                ConfigureServices = databaseContextManager.Register
            };
            server.Use(new CookieSessions<UserSession>(TimeSpan.FromDays(14))
            {
                Secure = cfg.Production,
                Path = "/",
                Store = new EntityFrameworkSessionStore<UserSession>(() => new NxplxContext(), session => session.User)
            });
            server.OnHandlerException += (sender, eventArgs) =>
            {
                logger.Error(
                    "Exception on url {ExceptionUrl}: {ExceptionType} with message {ExceptionMessage} :: {Stacktrace}",
                    eventArgs.Exception, eventArgs.Exception.GetType().Name, eventArgs.Exception.Message,
                    eventArgs.Exception.StackTrace);
            };

            await databaseContextManager.Initialize(logger);

            server.Get("/api/build", Authenticated.User, (req, res) => res.SendString(cfg.Build));
            
            server.CreateRouter("/api/authentication", AuthenticationRoutes.BindHandlers);
            server.CreateRouter("/api/user", UserRoutes.BindHandlers);
            server.CreateRouter("/api/session", SessionRoutes.BindHandlers);
            server.CreateRouter("/api/library", LibraryRoutes.BindHandlers);
            server.CreateRouter("/api/overview", OverviewRoutes.BindHandlers);
            server.CreateRouter("/api/series", EpisodeRoutes.BindHandlers);
            server.CreateRouter("/api/film", FilmRoutes.BindHandlers);
            
            server.CreateRouter("/api/indexing", IndexingRoutes.BindHandlers);
            server.CreateRouter("/api/websocket", BroadcastRoutes.BindHandlers);
            server.CreateRouter("/api/subtitle", SubtitleRoutes.BindHandlers);
            server.CreateRouter("/api/progress", ProgressRoutes.BindHandlers);
            server.CreateRouter("/api/image", ImageRoutes.BindHandlers);
            server.CreateRouter("/api/command", CommandRoutes.BindHandlers);

            server.Get("/*", Utils.SendSPA);

            logger.Trace("All routes registered, preparing to listen on port {Port}", cfg.HttpPort);

            await CreateAdminAccount(container);
            
            await server.RunAsync(cfg.Production ? "*" : "localhost");
        }

        
        private static async Task CreateAdminAccount(ResolveContainer container)
        {
            var logger = container.Resolve<ILoggingService>();
            await using var ctx = container.Resolve<IReadNxplxContext>();
            await using var transaction = ctx.BeginTransactionedContext();
            if (await ctx.Users.Many().CountAsync() == 0)
            {
                var admin = new User
                {
                    Username = "admin",
                    Admin = true,
                    Email = "",
                    PasswordHash = PasswordUtils.Hash("changemebaby")
                };
                transaction.Users.Add(admin);
                await transaction.SaveChanges();

                logger.Info("No users found. Default admin account created");
            }
        }
    }
}