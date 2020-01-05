using System;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Configuration;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.IoC.Conventions;
using NxPlx.Infrastructure.Session;
using NxPlx.Infrastructure.WebApi.Routes;
using NxPlx.Infrastructure.WebApi.Routes.Services;
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
        static async Task Main(string[] args)
        {
            WebApiConventions.Install();

            var cfg = ConfigurationService.Current;
            var container = ResolveContainer.Default();
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
                Store = new EntityFrameworkSessionStore<UserSession>(() => new NxplxContext())
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

            AuthenticationRoutes.Register(server.CreateRouter("/api/authentication"));
            UserRoutes.Register(server.CreateRouter("/api/user"));
            SessionRoutes.Register(server.CreateRouter("/api/session"));
            LibraryRoutes.Register(server.CreateRouter("/api/library"));
            OverviewRoutes.Register(server.CreateRouter("/api/overview"));
            EpisodeRoutes.Register(server.CreateRouter("/api/series"));
            FilmRoutes.Register(server.CreateRouter("/api/film"));

            IndexingRoutes.Register(server.CreateRouter("/api/indexing"));
            BroadcastRoutes.Register(server.CreateRouter("/api/broadcast"));
            SubtitleRoutes.Register(server.CreateRouter("/api/subtitle"));
            ProgressRoutes.Register(server.CreateRouter("/api/progress"));
            ImageRoutes.Register(server.CreateRouter("/api/image"));

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
            if (await ctx.Users.Count() == default)
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

                logger.Trace("No users found. Default admin account created");
            }
        }
    }
}