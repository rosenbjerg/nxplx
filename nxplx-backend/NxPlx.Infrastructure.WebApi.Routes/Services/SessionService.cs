using System.Collections.Generic;
using System.Threading.Tasks;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class SessionService
    {
        public static async Task<bool> CloseUserSession(string sessionId, int currentUserId, bool isAdmin)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
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
        public static async Task<List<UserSession>> GetUserSessions(int userId)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();

            var sessions = await context.UserSessions.Many(s => s.UserId == userId);
            return sessions;
        }
    }
}