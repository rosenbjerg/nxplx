using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Infrastructure.Broadcasting
{
    public class WebsocketConnectionAccepter : ConnectionAccepter
    {
        private readonly IOperationContext _operationContext;
        private readonly IServiceProvider _serviceProvider;

        public WebsocketConnectionAccepter(IOperationContext operationContext, ConnectionHub connectionHub, IServiceProvider serviceProvider) : base(connectionHub)
        {
            _operationContext = operationContext;
            _serviceProvider = serviceProvider;
        }

        public override async Task Accept(HttpContext httpContext)
        {
            Connection websocketConnection = new WebsocketConnection(httpContext, _operationContext);
            Connect(websocketConnection);
            await websocketConnection.KeepConnectionOpen(_operationContext.OperationCancelled);
            
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var user = await databaseContext.Users.SingleAsync(u => u.Id == _operationContext.Session.UserId);
            user.LastOnline = DateTime.UtcNow;
            await databaseContext.SaveChangesAsync();
        }
    }
}