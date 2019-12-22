using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class UserService
    {
        public static async Task UpdateUser(int userId, IFormCollection form)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
            await using var transaction = context.BeginTransactionedContext();

            var user = await transaction.Users.OneById(userId);

            if (form.TryGetValue("email", out var email))
            {
                user.Email = email;
            }

            await transaction.SaveChanges();
        }
        public static async Task<bool> RemoveUser(string username)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
            await using var transaction = context.BeginTransactionedContext();

            var user = await transaction.Users.One(u => u.Username == username);

            if (user == default) return false;

            transaction.Users.Remove(user);
            await transaction.SaveChanges();

            container.Resolve<ILoggingService>().Info("Deleted user {Username}", user.Username);
            return true;
        }
        public static async Task<bool> ChangeUserPassword(int userId, string oldPassword, string password1, string password2)
        {
            if (string.IsNullOrWhiteSpace(password1) || password1 != password2) return false;

            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
            await using var transaction = context.BeginTransactionedContext();
            var user = await transaction.Users.OneById(userId);

            if (user == null || !PasswordUtils.Verify(oldPassword, user.PasswordHash)) return false;

            user.PasswordHash = PasswordUtils.Hash(password1);
            user.HasChangedPassword = true;
            await transaction.SaveChanges();

            container.Resolve<ILoggingService>().Info("User {Username} changed password", user.Username);
            return true;
        }
        public static async Task<List<User>> GetUsers()
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
            var users = await context.Users.Many();
            return users;
        }
        public static async Task<User> GetUser(int userId)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();

            return await context.Users.OneById(userId);
        }
        public static async Task<User> CreateUser(string username, string email, bool isAdmin, IEnumerable<int> libraryIds, string password)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                Admin = isAdmin,
                LibraryAccessIds = libraryIds.ToList(),
                PasswordHash = PasswordUtils.Hash(password)
            };

            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
            await using var transaction = context.BeginTransactionedContext();

            transaction.Users.Add(user);
            await transaction.SaveChanges();

            container.Resolve<ILoggingService>()
                .Info("Created user {Username}", user.Username);
            return user;
        }
    }
}