using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using NxPlx.Models.Dto.Models;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class UserService
    {
        public static async Task UpdateUser(User user, IFormCollection form)
        {
            await using var context = ResolveContainer.Default.Resolve<IReadNxplxContext>(user);
            await using var transaction = context.BeginTransactionedContext();

            var existingUser = await transaction.Users.OneById(user.Id);
            if (form.TryGetValue("email", out var email))
            {
                existingUser.Email = email;
            }

            await transaction.SaveChanges();
        }
        public static async Task<bool> RemoveUser(User user, string username)
        {
            await using var context = ResolveContainer.Default.Resolve<IReadNxplxContext>(user);
            await using var transaction = context.BeginTransactionedContext();

            var existingUser = await transaction.Users.One(u => u.Username == username);
            if (existingUser == default) return false;

            transaction.Users.Remove(existingUser);
            await transaction.SaveChanges();

            ResolveContainer.Default.Resolve<ILoggingService>().Info("Deleted user {Username}", existingUser.Username);
            return true;
        }
        public static async Task<bool> ChangeUserPassword(User user, string oldPassword, string password1, string password2)
        {
            if (string.IsNullOrWhiteSpace(password1) || password1 != password2) return false;

            await using var context = ResolveContainer.Default.Resolve<IReadNxplxContext>(user);
            await using var transaction = context.BeginTransactionedContext();
            
            var existingUser = await transaction.Users.OneById(user.Id);
            if (existingUser == null || !PasswordUtils.Verify(oldPassword, existingUser.PasswordHash)) return false;

            existingUser.PasswordHash = PasswordUtils.Hash(password1);
            existingUser.HasChangedPassword = true;
            await transaction.SaveChanges();

            ResolveContainer.Default.Resolve<ILoggingService>().Info("User {Username} changed password", existingUser.Username);
            return true;
        }
        public static async Task<IEnumerable<UserDto>> GetUsers()
        {
            await using var context = ResolveContainer.Default.Resolve<IReadNxplxContext>();
            var users = await context.Users.Many();
            return ResolveContainer.Default.Resolve<IDtoMapper>().Map<User, UserDto>(users);
        }
        public static async Task<UserDto> GetUser(int userId)
        {
            await using var context = ResolveContainer.Default.Resolve<IReadNxplxContext>();

            var user = await context.Users.OneById(userId);
            return ResolveContainer.Default.Resolve<IDtoMapper>().Map<User, UserDto>(user);
        }
        public static async Task<UserDto> CreateUser(string username, string email, bool isAdmin, IEnumerable<int> libraryIds, string password)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                Admin = isAdmin,
                LibraryAccessIds = libraryIds.ToList(),
                PasswordHash = PasswordUtils.Hash(password)
            };

            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>();
            await using var transaction = context.BeginTransactionedContext();

            transaction.Users.Add(user);
            await transaction.SaveChanges();

            container.Resolve<ILoggingService>().Info("Created user {Username}", user.Username);
            return container.Resolve<IDtoMapper>().Map<User, UserDto>(user);
        }
    }
}