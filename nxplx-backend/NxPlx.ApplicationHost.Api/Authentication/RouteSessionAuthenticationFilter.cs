using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Sessions;
using NxPlx.Core.Services;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class RouteSessionAuthenticationFilter : IAsyncAuthorizationFilter
    {
        private readonly OperationContext _operationContext;
        private readonly IRouteSessionTokenExtractor _routeSessionTokenExtractor;
        private readonly IEventDispatcher _dispatcher;

        public RouteSessionAuthenticationFilter(OperationContext operationContext, IRouteSessionTokenExtractor routeSessionTokenExtractor, IEventDispatcher dispatcher)
        {
            _operationContext = operationContext;
            _routeSessionTokenExtractor = routeSessionTokenExtractor;
            _dispatcher = dispatcher;
        }
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var sessionToken = _routeSessionTokenExtractor.ExtractSessionToken(context.HttpContext.Request, "token");
            if (!string.IsNullOrEmpty(sessionToken))
            {
                var session = await _dispatcher.Dispatch(new SessionQuery(sessionToken));
                if (session != null)
                {
                    _operationContext.Session = session;
                    _operationContext.SessionId = sessionToken;
                    return;
                }
            }

            context.Result = new UnauthorizedResult();
        }
    }
}