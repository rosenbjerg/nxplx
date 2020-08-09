using System;
using NxPlx.Models;

namespace NxPlx.Application.Core
{
    public class OperationContext
    {
        public Guid CorrelationId { get; } = Guid.NewGuid();
        public User User { get; set; } = null!;
        public UserSession Session { get; set; } = null!;
    }
}