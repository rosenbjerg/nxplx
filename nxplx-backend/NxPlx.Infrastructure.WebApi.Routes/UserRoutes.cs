using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Session;
using NxPlx.Infrastructure.WebApi.Routes.Services;
using NxPlx.Models;
using Red;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class UserRoutes
    {
        public static void Register(IRouter router)
        {
            router.Post("/changepassword", Validated.ChangePasswordForm, Authenticated.User, ChangePassword);
            router.Get("", Authenticated.User, GetUser);
            router.Put("", Validated.UpdateUserDetailsForm, Authenticated.User, UpdateUser);
            
            router.Delete("", Authenticated.Admin, RemoveUser);
            router.Get("/list", Authenticated.Admin, ListUsers);
            router.Get("/online", Authenticated.Admin, ListUsers);
            router.Post("", Validated.CreateUserForm, Authenticated.Admin, CreateUser);
            
        }

        private static async Task<HandlerType> UpdateUser(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            var session = req.GetData<UserSession>();
            
            await UserService.UpdateUser(session.User, form);
            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> RemoveUser(Request req, Response res)
        {
            var username = req.ParseBody<JsonValue<string>>();
            var session = req.GetData<UserSession>();
            
            var ok = await UserService.RemoveUser(session.User, username.value);
            if (!ok) return await res.SendStatus(HttpStatusCode.BadRequest);
            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> ListUsers(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var users = await UserService.GetUsers(session.User);
            return await res.SendJson(users);
        }
        private static async Task<HandlerType> ListOnlineUsers(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var users = await UserService.ListOnlineUsers(session.User);
            return await res.SendJson(users);
        }
        private static async Task<HandlerType> GetUser(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var user = await UserService.GetUser(session.UserId);
            return await res.SendJson(user);
        }
        private static async Task<HandlerType> CreateUser(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            var user = await UserService.CreateUser(form["username"], form["email"], form["admin"] == "true", form["libraries"].Select(int.Parse), form["password1"]);
            return await res.SendJson(user);
        }
        private static async Task<HandlerType> ChangePassword(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            var session = req.GetData<UserSession>();

            var ok = await UserService.ChangeUserPassword(session.User, form["oldPassword"], form["password1"], form["password2"]);

            if (!ok) return await res.SendStatus(HttpStatusCode.BadRequest);
            return await res.SendStatus(HttpStatusCode.OK);
        }
    }
}