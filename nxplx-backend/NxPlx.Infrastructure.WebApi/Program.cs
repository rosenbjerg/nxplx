using System;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Configuration;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.IoC.Conventions;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Services.Database;
using NxPlx.WebApi.Routes;
using Red;
using Red.CookieSessions;
using Red.CookieSessions.EFCore;

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
                Store = new EntityFrameworkSessionStore<UserSession>(container.Resolve<UserContext>)
            });
            server.OnHandlerException += (sender, eventArgs) =>
            {
                Console.WriteLine(eventArgs.Exception.Message);
            }; 
            
            await databaseContextManager.Initialize(logger);
            
            CreateAdminAccount(container);
            
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
            
            
            await server.RunAsync(cfg.Production ? "*" : "localhost");
        }

        private static void CreateAdminAccount(ResolveContainer container)
        {
            using var ctx = container.Resolve<UserContext>();
            if (ctx.Users.FirstOrDefault() == null)
            {
                var admin = new User
                {
                    Username = "admin",
                    Admin = true,
                    Email = "",
                    PasswordHash = PasswordUtils.Hash("changemebaby")
                };
                ctx.Add(admin);
                ctx.SaveChanges();
            }
        }
    }
}