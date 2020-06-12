using NLog;
using NxPlx.Application.Core.Logging;
using NxPlx.Application.Models;

namespace NxPlx.Infrastructure.Logging
{
    public class NLoggingService : IStructuredLoggingService
    {
        private readonly Logger _logger;

        public NLoggingService(Logger logger)
        {
            _logger = logger;
        }

        public IStructuredLoggingService WithProperty(string key, object value)
        {
            return new NLoggingService(_logger.WithProperty(key, value));
        }

        public void Trace(string message, params object[] arguments) => _logger.Trace(message, arguments);

        public void Debug(string message, params object[] arguments) => _logger.Debug(message, arguments);

        public void Info(string message, params object[] arguments) => _logger.Info(message, arguments);

        public void Warn(string message, params object[] arguments) => _logger.Warn(message, arguments);

        public void Error(string message, params object[] arguments) => _logger.Error(message, arguments);

        public void Fatal(string message, params object[] arguments) => _logger.Fatal(message, arguments);
    }
}