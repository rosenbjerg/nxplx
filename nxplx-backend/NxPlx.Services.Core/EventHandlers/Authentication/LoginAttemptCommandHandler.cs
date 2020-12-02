using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Authentication;
using NxPlx.Application.Models.Events.Sessions;
using NxPlx.Infrastructure.Database;
using NxPlx.Models;

namespace NxPlx.Core.Services.EventHandlers.Authentication
{
    public class LoginAttemptCommandHandler : IEventHandler<LoginAttemptCommand, (string Token, DateTime Expiry, bool IsAdmin)>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly OperationContext _operationContext;
        private readonly IEventDispatcher _dispatcher;
        private readonly ILogger<LoginAttemptCommandHandler> _logger;
        private readonly TimeSpan _sessionLength;

        public LoginAttemptCommandHandler(DatabaseContext databaseContext, OperationContext operationContext, IConfiguration configuration, IEventDispatcher dispatcher, ILogger<LoginAttemptCommandHandler> logger)
        {
            _databaseContext = databaseContext;
            _operationContext = operationContext;
            _dispatcher = dispatcher;
            _logger = logger;
            var sessionConfig = configuration.GetSection("Session");
            _sessionLength = TimeSpan.FromDays(int.Parse(sessionConfig["LengthInDays"] ?? "20"));
        }

        public async Task<(string Token, DateTime Expiry, bool IsAdmin)> Handle(LoginAttemptCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == command.Username, cancellationToken);
            if (user != null && PasswordUtils.Verify(command.Password, user.PasswordHash))
            {
                var token = TokenGenerator.Generate();
                var expiry = DateTime.UtcNow.Add(_sessionLength);
                var session = new Session
                {
                    UserAgent = command.UserAgent,
                    IsAdmin = user.Admin,
                    UserId = user.Id,
                };
                await _dispatcher.Dispatch(new CreateSessionCommand(user.Id, token, session, _sessionLength));
                _logger.LogInformation("User {UserId} logged in", session.UserId);
                _operationContext.Session = session;
                return (token, expiry, user.Admin);
            }

            return default;
        }
    }
}