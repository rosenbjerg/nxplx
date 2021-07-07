using System.Threading.Tasks;
using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Application.Services;
using NxPlx.Domain.Events.Sessions;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class IntegratedHangfireAuthentication : IDashboardAsyncAuthorizationFilter
    {
        public async Task<bool> AuthorizeAsync(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var httpSessionService = httpContext.RequestServices.GetRequiredService<IHttpSessionService>();
            var dispatcher = httpContext.RequestServices.GetRequiredService<IEventDispatcher>();
            var sessionToken = httpSessionService.ExtractSessionToken(httpContext);
            if (!string.IsNullOrEmpty(sessionToken))
            {
                var session = await dispatcher.Dispatch(new SessionQuery(sessionToken));
                return session != null;
            }

            return false;
        }
    }
}