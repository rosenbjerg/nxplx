using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.Dto.Models;
using NxPlx.Services.Database;
using Red;
using Red.CookieSessions;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.WebApi.Routes
{
    public static class SessionRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("", Authenticated.User, GetSessions);
            router.Delete("", Validated.RequireSessionIdQuery, Authenticated.User, CloseSession);
            router.Get("/all", Validated.RequireUserIdQuery, Authenticated.Admin, GetUsersSessions);
        }

        private static async Task<HandlerType> GetSessions(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();

            var sessions = await context.UserSessions
                .Where(s => s.UserId == session.UserId)
                .OrderBy(s => s.Expiration)
                .ToListAsync();

            return await res.SendMapped<UserSession, UserSessionDto>(container.Resolve<IDatabaseMapper>(), sessions);
        }
        private static async Task<HandlerType> CloseSession(Request req, Response res)
        {
            var currentSession = req.GetData<UserSession>();
            string sessionId = req.Queries["sessionId"];
            
            if (currentSession.Id == sessionId)
            {
                await res.CloseSession(currentSession);
                return await res.SendStatus(HttpStatusCode.OK);
            }
            
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();

            var session = await context.UserSessions.FindAsync(sessionId);
            if (session.UserId != currentSession.UserId && !currentSession.IsAdmin)
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            context.UserSessions.Remove(session);
            await context.SaveChangesAsync();
            
            return await res.SendStatus(HttpStatusCode.OK);
        }

        private static async Task<HandlerType> GetUsersSessions(Request req, Response res)
        {
            var userId = int.Parse(req.Queries["userId"]);
            
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();

            var sessions = await context.UserSessions
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.Expiration)
                .ToListAsync();

            return await res.SendMapped<UserSession, UserSessionDto>(container.Resolve<IDatabaseMapper>(), sessions);
        }
    }
}