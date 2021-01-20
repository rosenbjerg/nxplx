using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core.Options;
using Serilog;
using Serilog.Filters;
using Serilog.Formatting.Json;

namespace NxPlx.ApplicationHost.Api
{
    public static class LoggingConfiguration
    {
        public static IHostBuilder UseNxplxSerilog(this IHostBuilder hostBuilder, string serviceName)
        {
            return hostBuilder
                .ConfigureServices((_, collection) => collection.AddLogging(builder => builder.ClearProviders()))
                .UseSerilog(ConfigureSerilog(serviceName));
        }

        private static Action<HostBuilderContext, LoggerConfiguration> ConfigureSerilog(string serviceName)
        {
            return (hostingContext, loggerConfiguration) =>
            {
                var logSettings = hostingContext.Configuration.GetSection("Logging").Get<LoggingOptions>();
                var enrichedConfiguration = loggerConfiguration
                    .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                    .Filter.ByExcluding(Matching.FromSource("Hangfire"))
                    .Destructure.ToMaximumDepth(3)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Service", serviceName);
                    
#if DEBUG
                enrichedConfiguration.WriteTo.Async(x => x.Console(logSettings.LogLevel));
#else
                enrichedConfiguration.WriteTo.Async(x => x.Console(new JsonFormatter(), logSettings.LogLevel));
#endif
                if (!string.IsNullOrEmpty(logSettings.Seq))
                    enrichedConfiguration.WriteTo.Seq(logSettings.Seq);
            };
        }
    }
}