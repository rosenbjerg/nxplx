namespace NxPlx.Application.Core.Logging
{
    public class SystemLogger : LoggerBase
    {
        public SystemLogger(ILoggerProvider loggerProvider, OperationContext operationContext)
            : base(loggerProvider
                .GetLoggingService("System")
                .WithProperty("CorrelationId", operationContext.Session))
        { }
    }
}