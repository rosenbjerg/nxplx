using System.Threading;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Domain.Events.File;
using NxPlx.Domain.Models.File;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.File
{
    public class StreamUrlQueryHandler : IDomainEventHandler<StreamUrlQuery, string>
    {
        private readonly IOperationContext _operationContext;

        public StreamUrlQueryHandler(IOperationContext operationContext)
        {
            _operationContext = operationContext;
        }
        
        public Task<string> Handle(StreamUrlQuery command, CancellationToken cancellationToken = default)
        {
            var streamKind = command.StreamKind.ToString().ToLowerInvariant();
            var result = $"{streamKind}/{_operationContext.SessionId}/{command.MediaFileId}";
            return Task.FromResult(result);
        }
    }
}