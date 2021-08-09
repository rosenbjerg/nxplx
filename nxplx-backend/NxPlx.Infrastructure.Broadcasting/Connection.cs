using System;
using System.Threading;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Application.Core;

namespace NxPlx.Infrastructure.Broadcasting
{
    public abstract class Connection
    {
        protected Connection(IOperationContext operationContext)
        {
            OperationContext = operationContext;
        }
        
        public IOperationContext OperationContext { get; }

        public abstract Task SendMessage(Message message);

        public abstract Task KeepConnectionOpen(CancellationToken cancellationToken);
        
        public event EventHandler<Message> MessageReceived = null!;
        public event EventHandler Disconnected = null!;

        protected void OnMessageReceived(Message message) => MessageReceived?.Invoke(this, message);
        protected void OnDisconnected() => Disconnected?.Invoke(this, EventArgs.Empty);
    }
}