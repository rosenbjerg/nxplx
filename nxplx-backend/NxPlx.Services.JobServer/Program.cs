using System;
using System.Threading.Tasks;
using Red;
using Hangfire;
using Hangfire.PostgreSql;
using NxPlx.Configuration;

namespace NxPlx.Jobs.Server
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            GlobalConfiguration.Configuration.UseNLogLogProvider();
            
            var options = new BackgroundJobServerOptions
            {
                Queues = new[] { "convert" }
            };

            var cfg = ConfigurationService.Current;
            
            var server = new RedHttpServer(cfg.JobServerPort)
            {
                ConfigureServices = services =>
                {
                    var connectionString = $"Host={cfg.SqlHost};Database={cfg.SqlJobDatabase};Username={cfg.SqlUsername};Password={cfg.SqlPassword}";
                    services.AddHangfire(config => config.UsePostgreSqlStorage(connectionString));
                },
                ConfigureApplication = application =>
                {
                    application.UseHangfireServer(options);
                    application.UseHangfireDashboard();
                }
            };
            
            Console.WriteLine("NxPlx.Services.JobServer starting...");
            await server.RunAsync();
        }
    }
}