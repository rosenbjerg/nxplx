namespace NxPlx.Application.Core.Logging
{
    public interface ILoggerProvider
    {
        public IStructuredLoggingService GetLoggingService(string name);
    }
}