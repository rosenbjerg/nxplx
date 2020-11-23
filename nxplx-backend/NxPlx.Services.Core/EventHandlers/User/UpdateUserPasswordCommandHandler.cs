using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events;
using NxPlx.Application.Models.Events.User;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.User
{
    public class UpdateUserPasswordCommandHandler : IEventHandler<UpdateUserPasswordCommand, bool>
    {
        private readonly DatabaseContext _context;
        private readonly IEventDispatcher _dispatcher;
        private readonly ILogger<UpdateUserPasswordCommandHandler> _logger;

        public UpdateUserPasswordCommandHandler(DatabaseContext context, IEventDispatcher dispatcher, ILogger<UpdateUserPasswordCommandHandler> logger)
        {
            _context = context;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateUserPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var currentUser = await _dispatcher.Dispatch(new CurrentUserQuery());
            if (string.IsNullOrWhiteSpace(command.NewPassword1) || command.NewPassword1 != command.NewPassword2) return false;

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUser.Id, cancellationToken: cancellationToken);
            if (existingUser == null || !PasswordUtils.Verify(command.OldPassword, existingUser.PasswordHash)) return false;

            existingUser.PasswordHash = PasswordUtils.Hash(command.NewPassword1);
            existingUser.HasChangedPassword = true;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {Username} changed password", existingUser.Username);
            return true;
        }
    }
}