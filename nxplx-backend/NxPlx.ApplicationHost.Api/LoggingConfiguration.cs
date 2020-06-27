using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core.Options;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Json;

namespace NxPlx.ApplicationHost.Api
{
    public static class LoggingConfiguration
    {
        public static IHostBuilder UseNxplxSerilog(this IHostBuilder hostBuilder, string serviceName)
        {
            return hostBuilder
                .ConfigureServices((context, collection) => collection.AddLogging(builder => builder.ClearProviders()))
                .UseSerilog(ConfigureSerilog(serviceName));
        }

        private static Action<HostBuilderContext, LoggerConfiguration> ConfigureSerilog(string serviceName)
        {
            return (hostingContext, loggerConfiguration) =>
            {
                var logSettings = hostingContext.Configuration.GetSection("Logging").Get<LoggingOptions>();
                var logDirectory = logSettings.Directory;
                loggerConfiguration.Filter.ByExcluding(Matching.FromSource("Microsoft"));
                loggerConfiguration.Filter.ByExcluding(Matching.FromSource("Hangfire"));
                loggerConfiguration.Enrich.FromLogContext()
                    .WriteTo.Async(x => x.File(
                        formatter: new JsonFormatter(),
                        path: Path.Combine(logDirectory, $"{serviceName}-{Environment.MachineName}-.log"),
                        restrictedToMinimumLevel: logSettings.LogLevel,
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: 50000000,
                        retainedFileCountLimit: 90,
                        buffered: true));
            };
        }
    }
}