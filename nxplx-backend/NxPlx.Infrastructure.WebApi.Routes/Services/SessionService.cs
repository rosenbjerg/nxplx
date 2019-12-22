using System.Collections.Generic;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models.Dto.Models;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class SessionService
    {
        public static async Task<bool> CloseUserSession(string sessionId, int currentUserId, bool isAdmin)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadContext>();
            await using var transaction = context.BeginTransactionedContext();

            var session = await transaction.UserSessions.OneById(sessionId);
            if (session.UserId != currentUserId && !isAdmin)
            {
                return false;
            }

            transaction.UserSessions.Remove(session);
            await transaction.SaveChanges();
            return true;
        }
        public static async Task<IEnumerable<UserSessionDto>> GetUserSessions(int userId)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadContext>();

            var sessions = await context.UserSessions.Many(s => s.UserId == userId);
            return container.Resolve<IDtoMapper>().Map<UserSession, UserSessionDto>(sessions);
        }
    }
}