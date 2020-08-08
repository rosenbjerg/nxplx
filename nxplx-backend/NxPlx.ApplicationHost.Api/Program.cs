using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace NxPlx.ApplicationHost.Api
{
    public class Program
    {
#if DEBUG
        private const string UseUrls = "http://localhost:5353";
#else
        private const string UseUrls = "http://*:5353";
#endif
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) => builder.AddEnvironmentVariables("NxPlx_"))
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .UseKestrel()
                    .UseStartup<Startup>()
                    .UseUrls(UseUrls))
                .UseNxplxSerilog("Api");
    }
}