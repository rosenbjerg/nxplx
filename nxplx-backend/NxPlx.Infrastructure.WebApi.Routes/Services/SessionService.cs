using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.Dto.Models;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class SessionService
    {
        public static async Task<bool> CloseUserSession(User user, string sessionId)
        {
            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>(user);
            await using var transaction = context.BeginTransactionedContext();

            var session = await transaction.UserSessions.OneById(sessionId);
            if (session == default || session.UserId != user.Id)
            {
                return false;
            }

            transaction.UserSessions.Remove(session);
            await transaction.SaveChanges();
            return true;
        }
        public static async Task<IEnumerable<UserSessionDto>> GetUserSessions(User user, int userId)
        {
            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>(user);

            var sessions = await context.UserSessions.Many(s => s.UserId == userId).ToListAsync();
            return container.Resolve<IDtoMapper>().Map<UserSession, UserSessionDto>(sessions);
        }
    }
}