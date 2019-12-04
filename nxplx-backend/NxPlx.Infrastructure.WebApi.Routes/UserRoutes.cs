using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.Dto.Models;
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
            router.Post("", Validated.CreateUserForm, Authenticated.Admin, CreateUser);
            
        }

        private static async Task<HandlerType> UpdateUser(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();

            var session = req.GetData<UserSession>();
            
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
            await using var transaction = context.BeginTransactionedContext();
            
            var user = await transaction.Users.OneById(session.UserId);

            if (form.TryGetValue("email", out var email))
            {
                user.Email = email;
            }

            await transaction.Commit();
            return await res.SendStatus(HttpStatusCode.OK);
        }

        private static async Task<HandlerType> RemoveUser(Request req, Response res)
        {
            var username = req.ParseBody<JsonValue<string>>().value;

            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
            await using var transaction = context.BeginTransactionedContext();

            var user = await transaction.Users.One(u => u.Username == username);

            if (user == default)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            transaction.Users.Remove(user);
            await transaction.Commit();

            container.Resolve<ILoggingService>()
                .Info("Deleted user {Username}", user.Username);

            return await res.SendStatus(HttpStatusCode.OK);
        }
        
        private static async Task<HandlerType> ListUsers(Request req, Response res)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
            var users = await context.Users.Many();

            var mapper = container.Resolve<IDatabaseMapper>();
            return await res.SendMapped<User, UserDto>(mapper, users);
        }
        
        private static async Task<HandlerType> GetUser(Request req, Response res)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
            var session = req.GetData<UserSession>();
            
            var user = await context.Users.OneById(session.UserId);

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
                PasswordHash = PasswordUtils.Hash(form["password1"])
            };

            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
            await using var transaction = context.BeginTransactionedContext();
            
            transaction.Users.Add(user);
            await transaction.Commit();

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
            await using var context = container.Resolve<IReadUserContext>();
            await using var transaction = context.BeginTransactionedContext();
            var user = await transaction.Users.OneById(session.UserId);

            if (user == null || !PasswordUtils.Verify(oldPassword, user.PasswordHash))
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            user.PasswordHash = PasswordUtils.Hash(password1);
            user.HasChangedPassword = true;
            await transaction.Commit();
            
            container.Resolve<ILoggingService>()
                .Info("User {Username} changed password", user.Username);

            
            return await res.SendStatus(HttpStatusCode.OK);
        }
    }
}