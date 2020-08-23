using NxPlx.Application.Core;
using Serilog.Core;
using Serilog.Events;

namespace NxPlx.ApplicationHost.Api.Logging
{
    public class CommonEventEnricher : ILogEventEnricher
    {
        private readonly OperationContext _operationContext;

        public CommonEventEnricher(OperationContext operationContext)
        {
            this._operationContext = operationContext;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(nameof(OperationContext.SessionId), _operationContext.SessionId));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(nameof(OperationContext.Session.UserId), _operationContext.Session));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(nameof(OperationContext.CorrelationId), _operationContext.CorrelationId.ToString()));
        }
    }
}