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
    public static class UserRoutes
    {
        public static void Register(IRouter router)
        {
            router.Post("/changepassword", Validated.ChangePasswordForm, Authenticated.User, ChangePassword);
            router.Get("", Authenticated.User, GetUser);
            router.Get("/list", Authenticated.Admin, ListUsers);
            router.Post("", Validated.CreateUserForm, Authenticated.Admin, CreateUser);
            router.Delete("", Authenticated.Admin, RemoveUser);
        }

        private static async Task<HandlerType> RemoveUser(Request req, Response res)
        {
            var username = req.ParseBody<JsonValue<string>>().value;

            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();

            var user = await context.Users.Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            context.Remove(user);
            await context.SaveChangesAsync();

            container.Resolve<ILoggingService>()
                .Info("Deleted user {Username}", user.Username);

            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> ListUsers(Request req, Response res)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();
            var users = await context.Users.ToListAsync();

            var mapper = container.Resolve<IDatabaseMapper>();
            return await res.SendMapped<User, UserDto>(mapper, users);
        }
        
        private static async Task<HandlerType> GetUser(Request req, Response res)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();
            var session = req.GetData<UserSession>();
            
            var user = await context.Users.FindAsync(session.UserId);

            return await res.SendMapped<User, UserDto>(container.Resolve<IDatabaseMapper>(), user);
        }
        
        private static async Task<HandlerType> CreateUser(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();

            var user = new User
            {
                Username = form["username"],
                Email = form["email"],
                Admin = form["admin"] == "true",
                LibraryAccessIds = form["libraries"].Select(int.Parse).ToList(),
                PasswordHash = PasswordUtils.Hash(form["password"])
            };

            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            container.Resolve<ILoggingService>()
                .Info("Created user {Username}", user.Username);

            return await res.SendMapped<User, UserDto>(container.Resolve<IDatabaseMapper>(), user);
        }
        
        private static async Task<HandlerType> ChangePassword(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();

            string oldPassword = form["oldPassword"];
            string password1 = form["password1"];
            string password2 = form["password2"];

            if (string.IsNullOrWhiteSpace(password1) || password1 != password2)
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            var session = req.GetData<UserSession>();

            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();
            var user = await context.Users.FindAsync(session.UserId);

            if (user == null || !PasswordUtils.Verify(oldPassword, user.PasswordHash))
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            user.PasswordHash = PasswordUtils.Hash(password1);
            user.HasChangedPassword = true;
            await context.SaveChangesAsync();
            
            container.Resolve<ILoggingService>()
                .Info("User {Username} changed password", user.Username);

            
            return await res.SendStatus(HttpStatusCode.OK);
        }
    }
}