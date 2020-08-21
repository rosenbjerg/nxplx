using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Core.Services;
using NxPlx.Models;
using NxPlx.Services.Database;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class SessionAuthenticationFilter : IAsyncAuthorizationFilter
    {
        private readonly OperationContext _operationContext;
        private readonly IHttpSessionService _sessionService;
        private readonly DatabaseContext _databaseContext;

        public SessionAuthenticationFilter(OperationContext operationContext, IHttpSessionService sessionService, DatabaseContext databaseContext)
        {
            _operationContext = operationContext;
            _sessionService = sessionService;
            _databaseContext = databaseContext;
        }

        private async Task<UserSession?> FindSession(HttpContext context)
        {
            var sessionToken = _sessionService.ExtractSessionToken(context.Request);

            if (!string.IsNullOrEmpty(sessionToken))
            {
                var session = await _databaseContext.UserSessions
                    .Where(s => s.Id == sessionToken).Include(s => s.User)
                    .AsNoTracking().FirstOrDefaultAsync();

                if (session != null)
                {
                    if (session.Expiration < DateTime.UtcNow)
                    {
                        _databaseContext.Remove(session);
                        await _databaseContext.SaveChangesAsync();
                    }
                    else
                    {
                        return session;
                    }
                }

                _sessionService.AttachSessionToken(context.Response, null);
            }

            return null;
        }
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var session = await FindSession(context.HttpContext);

            if (session != null)
            {
                _operationContext.User = session.User;
                _operationContext.Session = session;
            }
            else context.Result = new UnauthorizedResult();
        }
    }
}