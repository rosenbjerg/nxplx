using System;
using System.Collections.Generic;
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
using Red.Interfaces;
using Crypt = BCrypt.Net.BCrypt;

namespace NxPlx.WebApi.Routers
{
    public static class UserRoutes
    {
        const string NxSalt = "Nx";
        private static int _workfactor;
        public static void Register(IRouter router)
        {
            var container = new ResolveContainer();
            var (workFactor, timeUsed) = DetermineWorkFactor();
            {
                var logger = container.Resolve<ILoggingService>();
                logger.Info($"BCrypt Password workfactor set to {workFactor}, taking {timeUsed:F1}ms");
                _workfactor = workFactor;
            }

            using var ctx = container.Resolve<UserContext>();
            if (!ctx.Users.Any())
            {
                var admin = new User
                {
                    Username = "admin",
                    Admin = true,
                    Email = "",
                    LibraryAccessIds = new List<int> { 1, 2, 3, 4, 5, 6 },
                    PasswordHash = Crypt.HashPassword(NxSalt + "changemebaby", workFactor)
                };
                ctx.Add(admin);
                ctx.SaveChanges();
            }
            
            router.Post("/login", Login);
            router.Get("/verify", Authenticated.User, Verify);
            router.Post("/logout", Authenticated.User, Logout);
            router.Post("/changepassword", Authenticated.User, ChangePassword);
            router.Get("/libraries", Authenticated.User, ListLibraries);
            
            router.Get("/list", Authenticated.Admin, ListUsers);
            router.Post("/librarypermissions", Authenticated.Admin, SetUserLibraryPermissions);
            router.Post("/create", Authenticated.Admin, CreateUser);
            router.Delete("/remove", Authenticated.Admin, RemoveUser);
        }

        private static Task<HandlerType> Verify(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            return res.SendString(session.IsAdmin.ToString());
        }

        private static async Task<HandlerType> RemoveUser(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            string username = form["username"];

            var container = new ResolveContainer();
            await using var context = container.Resolve<UserContext>();

            var user = await context.Users.Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            context.Remove(user);
            await context.SaveChangesAsync();

            var logger = container.Resolve<ILoggingService>();
            logger.Info($"Deleted user {user.Username}");

            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> ListUsers(Request req, Response res)
        {
            var container = new ResolveContainer();
            await using var context = container.Resolve<UserContext>();
            var users = await context.Users.ToListAsync();

            var mapper = container.Resolve<IDatabaseMapper>();
            return await res.SendJson(mapper.MapMany<User, UserDto>(users));
        }
        private static async Task<HandlerType> ListLibraries(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var container = new ResolveContainer();
            await using var context = container.Resolve<MediaContext>();
            var libraries = await context.Libraries.ToListAsync();

            var mapper = container.Resolve<IDatabaseMapper>();
            
            if (session.IsAdmin)
                return await res.SendJson(mapper.MapMany<Library, AdminLibraryDto>(libraries));
            else
                return await res.SendJson(mapper.MapMany<Library, LibraryDto>(libraries));
        }
        private static async Task<HandlerType> SetUserLibraryPermissions(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();

            var userId = int.Parse(form["userId"]);
            var libraryIds = form["libraries"].Select(int.Parse).ToList();
            
            var container = new ResolveContainer();
            await using var context = container.Resolve<UserContext>();
            await using var mediaContext = container.Resolve<MediaContext>();
            
            var user = await context.Users.FindAsync(userId);

            if (user == default)
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }
            
            var libraries = await mediaContext.Libraries
                .Where(l => libraryIds.Contains(l.Id))
                .ToListAsync();
            
            if (libraryIds.Count != libraries.Count)
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            user.LibraryAccessIds.Clear();
            user.LibraryAccessIds.AddRange(libraryIds);
            await context.SaveChangesAsync();
            
            var mapper = container.Resolve<IDatabaseMapper>();
            return await res.SendJson(mapper.MapMany<Library, LibraryDto>(libraries));
        }
        private static async Task<HandlerType> CreateUser(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            var password = Guid.NewGuid().ToString("N");

            var user = new User
            {
                Username = form["username"],
                Email = form["email"],
                Admin = form["admin"] == "true",
                LibraryAccessIds = form["libraries"].Select(int.Parse).ToList(),
                PasswordHash = Crypt.HashPassword($"{NxSalt}{password}", _workfactor)
            };

            var container = new ResolveContainer();
            await using var context = container.Resolve<MediaContext>();
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var logger = container.Resolve<ILoggingService>();
            logger.Info($"Created user {user.Username} with access to libraries: {string.Join(' ', user.LibraryAccessIds)}");

            return await res.SendString(password);
        }

        private static async Task<HandlerType> Logout(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            await res.CloseSession(session);
            return await res.SendStatus(HttpStatusCode.OK);
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

            var container = new ResolveContainer();
            await using var context = container.Resolve<UserContext>();
            var user = await context.Users.FindAsync(session.UserId);

            if (user == null || !Crypt.Verify(NxSalt + oldPassword, user.PasswordHash))
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            user.PasswordHash = Crypt.HashPassword(NxSalt + password1);
            await context.SaveChangesAsync();

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

            var container = new ResolveContainer();
            await using var context = container.Resolve<UserContext>();
            var user = await context.Users.Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            if (user == null || !Crypt.Verify(NxSalt + password, user.PasswordHash))
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            await res.OpenSession(new UserSession
            {
                UserAgent = req.Headers["User-Agent"], IsAdmin = user.Admin, UserId = user.Id,
                LibraryAccess = user.LibraryAccessIds
            });

            return await res.SendStatus(HttpStatusCode.OK);
        }

        private static (int, double) DetermineWorkFactor()
        {
            var testValue = "jroif87twhgp34958h03";
            var workFactor = 10;
            double timeUsed;
            
            while ((timeUsed = TimeBCrypt(testValue, workFactor++)) < 300) ;

            return (workFactor, timeUsed);
        }

        private static double TimeBCrypt(string password, int workFactor)
        {
            var started = DateTime.UtcNow;
            var bcrypted = BCrypt.Net.BCrypt.HashPassword(password, workFactor);
            return DateTime.UtcNow.Subtract(started).TotalMilliseconds;
        }
    }
}