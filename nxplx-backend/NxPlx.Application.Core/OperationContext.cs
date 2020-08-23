using System;
using NxPlx.Models;

namespace NxPlx.Application.Core
{
    public class OperationContext
    {
        public Guid CorrelationId { get; } = Guid.NewGuid();
        public Session? Session { get; set; }
        public string? SessionId { get; set; }
    }
}