namespace NxPlx.Application.Core.Logging
{
    public interface IStructuredLoggingService : ILoggingService
    {
        public IStructuredLoggingService WithProperty(string key, object value);
    }
}