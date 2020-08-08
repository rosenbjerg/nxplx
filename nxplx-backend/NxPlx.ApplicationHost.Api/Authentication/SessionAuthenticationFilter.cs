using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Application.Core;
using NxPlx.Core.Services;
using NxPlx.Models;
using NxPlx.Services.Database;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class SessionAuthenticationFilter : IAsyncAuthorizationFilter
    {
        internal static async Task<UserSession?> FindSession(HttpContext context)
        {
            var sessionService = context.RequestServices.GetService<IHttpSessionService>();
            var sessionToken = sessionService.ExtractSessionToken(context.Request);

            if (!string.IsNullOrEmpty(sessionToken))
            {
                var databaseContext = context.RequestServices.GetService<DatabaseContext>();
                var session = await databaseContext.UserSessions
                    .Where(s => s.Id == sessionToken).Include(s => s.User)
                    .AsNoTracking().FirstOrDefaultAsync();

                if (session != null)
                {
                    if (session.Expiration < DateTime.UtcNow)
                    {
                        databaseContext.Remove(session);
                        await databaseContext.SaveChangesAsync();
                    }
                    else
                    {
                        return session;
                    }
                }

                sessionService.AttachSessionToken(context.Response, null);
            }

            return null;
        }
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var session = await FindSession(context.HttpContext);

            if (session != null)
            {
                var operationContext = context.HttpContext.RequestServices.GetService<OperationContext>();
                operationContext.User = session.User;
                operationContext.Session = session;
            }
            else context.Result = new UnauthorizedResult();
        }
    }
}