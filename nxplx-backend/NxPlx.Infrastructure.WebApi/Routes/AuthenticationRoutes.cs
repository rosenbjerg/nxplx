using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Services.Database;
using Red;
using Red.CookieSessions;
using Red.Interfaces;

namespace NxPlx.WebApi.Routes
{
    public static class AuthenticationRoutes
    {
        public static void Register(IRouter router)
        {
            router.Post("/login", Validated.LoginForm, Login);
            router.Get("/verify", Authenticated.User, Verify);
            router.Post("/logout", Authenticated.User, Logout);
        }

        private static Task<HandlerType> Verify(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            return res.SendString(session.IsAdmin.ToString());
        }
        
        private static async Task<HandlerType> Logout(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            await res.CloseSession(session);
            return await res.SendStatus(HttpStatusCode.OK);
        }

        private static async Task<HandlerType> Login(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();

            string username = form["username"];
            string password = form["password"];

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();
            var user = await context.Users.Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            if (user == null || !PasswordUtils.Verify(password, user.PasswordHash))
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            await res.OpenSession(new UserSession
            {
                UserAgent = req.Headers["User-Agent"], 
                IsAdmin = user.Admin, 
                UserId = user.Id,
                LibraryAccess = user.LibraryAccessIds ?? new List<int>(0)
            });

            return await res.SendString(user.Admin.ToString());
        }

    }
}