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
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.WebApi.Routes
{
    public static class SessionRoutes
    {
        public static void Register(IRouter router)
        {
            router.Post("", Authenticated.User, GetSessions);
            router.Post("/all", Validated.RequireUserIdQuery, Authenticated.Admin, GetUsersSessions);
        }

        private static async Task<HandlerType> GetSessions(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();

            var sessions = await context.UserSessions
                .Where(s => s.UserId == session.UserId)
                .OrderBy(s => s.UserAgent)
                .ToListAsync();

            return await res.SendJson(sessions);
        }

        private static async Task<HandlerType> GetUsersSessions(Request req, Response res)
        {
            var userId = int.Parse(req.Queries["userId"]);
            
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();

            var sessions = await context.UserSessions
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.UserAgent)
                .ToListAsync();

            return await res.SendJson(sessions);
        }
    }
}