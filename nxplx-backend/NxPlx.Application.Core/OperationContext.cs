using System;
using System.Threading;
using NxPlx.Abstractions;
using NxPlx.Domain.Models;

namespace NxPlx.Application.Core
{
    public class OperationContext : IOperationContext
    {
        public CancellationToken OperationCancelled { get; set; }
        public Guid CorrelationId { get; } = Guid.NewGuid();
        public Session Session { get; set; } = null!;
        public string SessionId { get; set; } = null!;
    }
}