using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NxPlx.Application.Core;

namespace NxPlx.ApplicationHost.Api.Middleware
{
    public class OperationContextMiddleware
    {
        private readonly RequestDelegate _next;

        public OperationContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, OperationContext operationContext)
        {
            operationContext.OperationCancelled = context.RequestAborted;
            await _next(context);
        }
    }
}