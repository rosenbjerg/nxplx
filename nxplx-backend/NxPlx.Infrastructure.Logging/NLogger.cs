using System;
using System.IO;
using NLog;
using NLog.Targets;
using NxPlx.Configuration;

namespace NxPlx.Infrastructure.Logging
{
    public class NLogger : ILogger
    {
        private readonly Logger _logger = LogManager.GetLogger("");

        public NLogger()
        {
            var cfg = ConfigurationService.Current;
            
            var config = new NLog.Config.LoggingConfiguration();
            
            var logfile = new FileTarget("logfile")
            {
                ArchiveFileName = Path.Combine(cfg.LogFolder, "archives/log.{#####}.txt"),
                ArchiveAboveSize = 5000000,
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
                MaxArchiveFiles = 50
            };
            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            
            #if DEBUG
            var logconsole = new ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);
            #endif
            
            
            LogManager.Configuration = config;
        }

        public void Trace(string message, params object[] arguments) => _logger.Trace(message, arguments);

        public void Debug(string message, params object[] arguments) => _logger.Debug(message, arguments);

        public void Info(string message, params object[] arguments) => _logger.Info(message, arguments);

        public void Warn(string message, params object[] arguments) => _logger.Warn(message, arguments);

        public void Error(string message, params object[] arguments) => _logger.Error(message, arguments);

        public void Fatal(string message, params object[] arguments) => _logger.Fatal(message, arguments);
    }
}