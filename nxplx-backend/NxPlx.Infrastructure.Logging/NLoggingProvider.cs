using System.IO;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NxPlx.Application.Core.Logging;
using NxPlx.Application.Core.Settings;

namespace NxPlx.Infrastructure.Logging
{
    public class NLoggingProvider : ILoggerProvider
    {
        public NLoggingProvider(FolderSettings settings)
        {
            Directory.CreateDirectory(settings.Logs);

            var config = new LoggingConfiguration();

            var logfile = new AsyncTargetWrapper
            {
                WrappedTarget = new FileTarget("logfile")
                {
                    Layout = new JsonLayout
                    {
                        Attributes =
                        {
                            new JsonAttribute("Logger", Layout.FromString("${logger}")),
                            new JsonAttribute("Time", Layout.FromString("${longdate}")),
                            new JsonAttribute("Level", Layout.FromString("${level}")),
                            new JsonAttribute("Message", Layout.FromString("${message}")),
                        },
                        IncludeAllProperties = true
                    },
                    FileName = Path.Combine(settings.Logs, "log.current.json"),
                    ArchiveFileName = Path.Combine(settings.Logs, "archives", "log.{#}.json"),
                    ArchiveAboveSize = 5000000,
                    ArchiveEvery = FileArchivePeriod.Day,
                    ArchiveNumbering = ArchiveNumberingMode.Date,
                    MaxArchiveFiles = 50,
                    OptimizeBufferReuse = true
                }
            };

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

#if DEBUG
            var logconsole = new ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);
#endif

            LogManager.Configuration = config;
        }

        public IStructuredLoggingService GetLoggingService(string name)
        {
            return new NLoggingService(LogManager.GetLogger(name));
        }
    }
}