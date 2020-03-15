using System.Net;
using System.Threading.Tasks;
using NxPlx.Core.Services;
using NxPlx.Infrastructure.Session;
using Red;
using Red.CookieSessions;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class SessionRoutes
    {
        public static void BindHandlers(IRouter router)
        {
            router.Get("", Authenticated.User, GetSessions);
            router.Delete("", Validated.RequireSessionIdQuery, Authenticated.User, CloseSession);
            router.Get("/all", Validated.RequireUserIdQuery, Authenticated.Admin, GetUsersSessions);
        }

        private static async Task<HandlerType> GetSessions(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var sessions = await SessionService.GetUserSessions(session!.User, session!.User.Id);
            return await res.SendJson(sessions);
        }
        private static async Task<HandlerType> CloseSession(Request req, Response res)
        {
            var currentSession = req.GetData<UserSession>();
            string sessionId = req.Queries["sessionId"];
            
            if (currentSession!.Id == sessionId)
            {
                await res.CloseSession(currentSession);
                return await res.SendStatus(HttpStatusCode.OK);
            }
            
            var ok = await SessionService.CloseUserSession(currentSession!.User, sessionId);

            if (!ok) return await res.SendStatus(HttpStatusCode.BadRequest);
            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> GetUsersSessions(Request req, Response res)
        {
            var userId = int.Parse(req.Queries["userId"]);
            var currentSession = req.GetData<UserSession>();
            var sessions = await SessionService.GetUserSessions(currentSession!.User, userId);
            return await res.SendJson(sessions);
        }

    }
}