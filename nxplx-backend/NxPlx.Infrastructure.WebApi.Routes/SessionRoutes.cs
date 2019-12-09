using System.Net;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models.Dto.Models;
using Red;
using Red.CookieSessions;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
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
            await using var context = container.Resolve<IReadUserContext>();

            var sessions = await context.UserSessions.Many(s => s.UserId == session.UserId);

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
            await using var context = container.Resolve<IReadUserContext>();
            await using var transaction = context.BeginTransactionedContext();

            var session = await transaction.UserSessions.OneById(sessionId);
            if (session.UserId != currentSession.UserId && !currentSession.IsAdmin)
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            transaction.UserSessions.Remove(session);
            await transaction.SaveChanges();
            
            return await res.SendStatus(HttpStatusCode.OK);
        }

        private static async Task<HandlerType> GetUsersSessions(Request req, Response res)
        {
            var userId = int.Parse(req.Queries["userId"]);
            
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();

            var sessions = await context.UserSessions.Many(s => s.UserId == userId);

            return await res.SendMapped<UserSession, UserSessionDto>(container.Resolve<IDatabaseMapper>(), sessions);
        }
    }
}