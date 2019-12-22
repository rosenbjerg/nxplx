using System.Net;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Session;
using NxPlx.Infrastructure.WebApi.Routes.Services;
using Red;
using Red.CookieSessions;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
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

            var session = await AuthenticationService.TryCreateSession(form["username"], form["password"], req.Headers["User-Agent"]);
            if (session == null) return await res.SendStatus(HttpStatusCode.BadRequest);
            
            await res.OpenSession(session);
            return await res.SendString(session.IsAdmin.ToString());
        }
        
    }
}