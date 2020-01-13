using System.Threading.Tasks;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class AuthenticationService
    {
        public static async Task<UserSession?> TryCreateSession(string username, string password, string userAgent)
        {
            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>();
            var user = await context.Users.One(u => u.Username == username);

            if (user != null && PasswordUtils.Verify(password, user.PasswordHash))
            {
                return new UserSession
                {
                    UserAgent = userAgent,
                    IsAdmin = user.Admin,
                    UserId = user.Id
                };
            }

            return default;
        }
    }
}