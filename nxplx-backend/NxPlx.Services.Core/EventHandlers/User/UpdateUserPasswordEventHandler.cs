using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.User
{
    public class UpdateUserPasswordEventHandler : IEventHandler<UpdateUserPasswordEvent, bool>
    {
        private readonly DatabaseContext _context;
        private readonly UserContextService _userContextService;
        private readonly ILogger<UpdateUserPasswordEventHandler> _logger;

        public UpdateUserPasswordEventHandler(DatabaseContext context, UserContextService userContextService, ILogger<UpdateUserPasswordEventHandler> logger)
        {
            _context = context;
            _userContextService = userContextService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateUserPasswordEvent @event, CancellationToken cancellationToken = default)
        {
            var currentUser = await _userContextService.GetUser();
            if (string.IsNullOrWhiteSpace(@event.NewPassword1) || @event.NewPassword1 != @event.NewPassword2) return false;

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUser.Id, cancellationToken: cancellationToken);
            if (existingUser == null || !PasswordUtils.Verify(@event.OldPassword, existingUser.PasswordHash)) return false;

            existingUser.PasswordHash = PasswordUtils.Hash(@event.NewPassword1);
            existingUser.HasChangedPassword = true;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {Username} changed password", existingUser.Username);
            return true;
        }
    }
}