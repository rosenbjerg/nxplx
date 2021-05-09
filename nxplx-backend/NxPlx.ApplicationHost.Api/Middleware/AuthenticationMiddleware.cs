using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NxPlx.Application.Core;
using NxPlx.Application.Services;
using NxPlx.Domain.Events.Sessions;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, OperationContext operationContext, IRouteSessionTokenExtractor routeSessionTokenExtractor, IApplicationEventDispatcher dispatcher)
        {
            var sessionToken = routeSessionTokenExtractor.ExtractSessionToken(context.Request, "token");
            if (!string.IsNullOrEmpty(sessionToken))
            {
                var session = await dispatcher.Dispatch(new SessionQuery(sessionToken));
                if (session != null)
                {
                    operationContext.Session = session;
                    operationContext.SessionId = sessionToken;
                }
            }

            await _next(context);
        }
    }
}