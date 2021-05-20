using System;
using System.Threading;
using NxPlx.Domain.Models;

namespace NxPlx.Abstractions
{
    public interface IOperationContext
    {
        public CancellationToken OperationCancelled { get; }
        public Guid CorrelationId { get; }
        public Session Session { get; }
        public string SessionId { get; }
    }
}