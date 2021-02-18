using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Sessions;
using NxPlx.Application.Services;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class SessionAuthenticationFilter : IAsyncAuthorizationFilter
    {
        private readonly OperationContext _operationContext;
        private readonly IHttpSessionService _httpSessionService;
        private readonly IApplicationEventDispatcher _dispatcher;

        public SessionAuthenticationFilter(OperationContext operationContext, IHttpSessionService httpSessionService, IApplicationEventDispatcher dispatcher)
        {
            _operationContext = operationContext;
            _httpSessionService = httpSessionService;
            _dispatcher = dispatcher;
        }
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var sessionToken = _httpSessionService.ExtractSessionToken(context.HttpContext);
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