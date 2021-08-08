using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Application.Core.Options;
using NxPlx.Application.Events.Authentication;
using NxPlx.Domain.Events.Sessions;
using NxPlx.Domain.Models;
using NxPlx.Domain.Services;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers.Authentication
{
    public class LoginAttemptCommandHandler : IApplicationEventHandler<LoginAttemptCommand, (string Token, DateTime Expiry, bool IsAdmin)>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly OperationContext _operationContext;
        private readonly IApplicationEventDispatcher _dispatcher;
        private readonly ILogger<LoginAttemptCommandHandler> _logger;
        private readonly TimeSpan _sessionLength;

        public LoginAttemptCommandHandler(DatabaseContext databaseContext, OperationContext operationContext, SessionOptions sessionOptions, IApplicationEventDispatcher dispatcher, ILogger<LoginAttemptCommandHandler> logger)
        {
            _databaseContext = databaseContext;
            _operationContext = operationContext;
            _dispatcher = dispatcher;
            _logger = logger;
            _sessionLength = TimeSpan.FromDays(sessionOptions.LengthInDays ?? 20);
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
                await _dispatcher.Dispatch(new CreateSessionCommand(session.UserId, token, session, _sessionLength));
                
                user.LastLogin = DateTime.UtcNow;
                await _databaseContext.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("User {UserId} logged in", user.Id);
                _operationContext.Session = session;
                return (token, expiry, user.Admin);
            }

            return default;
        }
    }
}