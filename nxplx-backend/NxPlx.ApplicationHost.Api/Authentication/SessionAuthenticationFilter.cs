using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NxPlx.Application.Core;
using NxPlx.Core.Services;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class SessionAuthenticationFilter : IAsyncAuthorizationFilter
    {
        private readonly OperationContext _operationContext;
        private readonly IHttpSessionService _httpSessionService;
        private readonly SessionService _sessionService;

        public SessionAuthenticationFilter(OperationContext operationContext, IHttpSessionService httpSessionService, SessionService sessionService)
        {
            _operationContext = operationContext;
            _httpSessionService = httpSessionService;
            _sessionService = sessionService;
        }
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var sessionToken = _httpSessionService.ExtractSessionToken(context.HttpContext.Request);
            if (!string.IsNullOrEmpty(sessionToken))
            {
                var session = await _sessionService.FindSession(sessionToken, context.HttpContext.RequestAborted);
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