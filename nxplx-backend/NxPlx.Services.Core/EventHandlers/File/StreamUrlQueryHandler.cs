using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.File;

namespace NxPlx.Core.Services.EventHandlers.File
{
    public class StreamUrlQueryHandler : IEventHandler<StreamUrlQuery, string>
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