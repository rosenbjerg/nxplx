using NxPlx.Application.Core;
using Serilog.Core;
using Serilog.Events;

namespace NxPlx.ApplicationHost.Api.Logging
{
    public class CommonEventEnricher : ILogEventEnricher
    {
        private readonly IOperationContext _operationContext;

        public CommonEventEnricher(IOperationContext IOperationContext)
        {
            this._operationContext = IOperationContext;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(nameof(IOperationContext.SessionId), _operationContext.SessionId));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(nameof(IOperationContext.Session.UserId), _operationContext.Session));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(nameof(IOperationContext.CorrelationId), _operationContext.CorrelationId.ToString()));
        }
    }
}