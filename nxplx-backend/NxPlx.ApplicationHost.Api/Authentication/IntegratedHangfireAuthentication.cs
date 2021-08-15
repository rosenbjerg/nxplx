using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Application.Services;
using NxPlx.Domain.Events.Sessions;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class IntegratedHangfireAuthentication : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var httpSessionService = httpContext.RequestServices.GetRequiredService<IHttpSessionService>();
            var dispatcher = httpContext.RequestServices.GetRequiredService<IEventDispatcher>();
            var sessionToken = httpSessionService.ExtractSessionToken(httpContext);
            if (!string.IsNullOrEmpty(sessionToken))
            {
                var session = dispatcher.Dispatch(new SessionQuery(sessionToken)).GetAwaiter().GetResult();
                return session is { IsAdmin: true };
            }

            return false;
        }
    }
}