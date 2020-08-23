using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NxPlx.Application.Core;
using NxPlx.Models;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services
{
    public class AuthenticationService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly OperationContext _operationContext;
        private readonly SessionService _sessionService;
        private readonly TimeSpan _sessionLength;

        public AuthenticationService(DatabaseContext databaseContext, OperationContext operationContext, IConfiguration configuration, SessionService sessionService)
        {
            _databaseContext = databaseContext;
            _operationContext = operationContext;
            _sessionService = sessionService;
            var sessionConfig = configuration.GetSection("Session");
            _sessionLength = TimeSpan.FromDays(int.Parse(sessionConfig["LengthInDays"] ?? "20"));
        }
        public async Task<(string Token, DateTime Expiry, bool IsAdmin)> Login(string username, string password, string userAgent)
        {
            var user = await _databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
            if (user != null && PasswordUtils.Verify(password, user.PasswordHash))
            {
                var token = GenerateToken();
                var expiry = DateTime.UtcNow.Add(_sessionLength);
                var session = new Session
                {
                    UserAgent = userAgent,
                    IsAdmin = user.Admin,
                    UserId = user.Id,
                };
                await _sessionService.AddSession(user.Id, token, session, _sessionLength);
                _operationContext.Session = session;
                return (token, expiry, user.Admin);
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