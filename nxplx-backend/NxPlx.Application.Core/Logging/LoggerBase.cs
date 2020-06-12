namespace NxPlx.Application.Core.Logging
{
    public abstract class LoggerBase : ILoggingService
    {
        protected readonly ILoggingService Logger;

        protected LoggerBase(ILoggingService logger)
        {
            Logger = logger;
        }

        public void Trace(string message, params object[] arguments) => Logger.Trace(message, arguments);

        public void Debug(string message, params object[] arguments) => Logger.Debug(message, arguments);

        public void Info(string message, params object[] arguments) => Logger.Info(message, arguments);

        public void Warn(string message, params object[] arguments) => Logger.Warn(message, arguments);

        public void Error(string message, params object[] arguments) => Logger.Error(message, arguments);

        public void Fatal(string message, params object[] arguments) => Logger.Fatal(message, arguments);
    }
}