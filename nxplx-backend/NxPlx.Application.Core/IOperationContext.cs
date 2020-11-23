using System;
using System.Threading;
using NxPlx.Models;

namespace NxPlx.Application.Core
{
    public interface IOperationContext
    {
        public CancellationToken OperationCancelled { get; }
        public Guid CorrelationId { get; }
        public Session Session { get; }
        public string SessionId { get; }
    }
}