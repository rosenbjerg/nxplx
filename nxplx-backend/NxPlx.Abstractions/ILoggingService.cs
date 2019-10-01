namespace NxPlx.Abstractions
{
    public interface ILoggingService
    {
        void Trace(string message, params object[] arguments);
        void Debug(string message, params object[] arguments);
        void Info(string message, params object[] arguments);
        void Warn(string message, params object[] arguments);
        void Error(string message, params object[] arguments);
        void Fatal(string message, params object[] arguments);
    }
}