using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Models;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services
{
    public class AuthenticationService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly OperationContext _operationContext;

        public AuthenticationService(DatabaseContext databaseContext, OperationContext operationContext)
        {
            _databaseContext = databaseContext;
            _operationContext = operationContext;
        }
        public async Task<UserSession?> Login(string username, string password, string userAgent)
        {
            var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null && PasswordUtils.Verify(password, user.PasswordHash))
            {
                var session = new UserSession
                {
                    Id = GenerateToken(),
                    UserAgent = userAgent,
                    IsAdmin = user.Admin,
                    UserId = user.Id
                };
                _operationContext.User = user;
                _databaseContext.UserSessions.Add(session);
                await _databaseContext.SaveChangesAsync();
            }

            return default;
        }

        public async Task Logout()
        {
            _databaseContext.Remove(_operationContext.Session);
            await _databaseContext.SaveChangesAsync();
        }
        
        private static string GenerateToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[18];
            rng.GetBytes(bytes);
            
            return WebEncoders.Base64UrlEncode(bytes);
        }
    }
}