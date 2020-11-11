using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;
using NxPlx.Models;

namespace NxPlx.Core.Services.EventHandlers.Authentication
{
    public class LoginAttemptEventHandler : IEventHandler<LoginAttemptEvent, (string Token, DateTime Expiry, bool IsAdmin)>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly OperationContext _operationContext;
        private readonly SessionService _sessionService;
        private readonly TimeSpan _sessionLength;

        public LoginAttemptEventHandler(DatabaseContext databaseContext, OperationContext operationContext, IConfiguration configuration, SessionService sessionService)
        {
            _databaseContext = databaseContext;
            _operationContext = operationContext;
            _sessionService = sessionService;
            var sessionConfig = configuration.GetSection("Session");
            _sessionLength = TimeSpan.FromDays(int.Parse(sessionConfig["LengthInDays"] ?? "20"));
        }

        public async Task<(string Token, DateTime Expiry, bool IsAdmin)> Handle(LoginAttemptEvent @event, CancellationToken cancellationToken = default)
        {
            var user = await _databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == @event.Username);
            if (user != null && PasswordUtils.Verify(@event.Password, user.PasswordHash))
            {
                var token = TokenGenerator.Generate();
                var expiry = DateTime.UtcNow.Add(_sessionLength);
                var session = new Session
                {
                    UserAgent = @event.UserAgent,
                    IsAdmin = user.Admin,
                    UserId = user.Id,
                };
                await _sessionService.AddSession(user.Id, token, session, _sessionLength);
                _operationContext.Session = session;
                return (token, expiry, user.Admin);
            }

            return default;
        }
    }
}