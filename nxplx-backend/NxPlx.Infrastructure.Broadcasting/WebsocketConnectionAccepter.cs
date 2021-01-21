using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NxPlx.Application.Core;

namespace NxPlx.Infrastructure.Broadcasting
{
    public class WebsocketConnectionAccepter : ConnectionAccepter
    {
        private readonly IOperationContext _operationContext;

        public WebsocketConnectionAccepter(IOperationContext operationContext, ConnectionHub connectionHub) : base(connectionHub)
        {
            _operationContext = operationContext;
        }

        public override Task Accept(HttpContext httpContext)
        {
            var websocketConnection = new WebsocketConnection(httpContext, _operationContext);
            Connect(websocketConnection);
            return websocketConnection.KeepConnectionOpen();
        }
    }
}