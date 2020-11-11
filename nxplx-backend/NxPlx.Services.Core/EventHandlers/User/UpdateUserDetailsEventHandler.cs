using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.User
{
    public class UpdateUserDetailsEventHandler : IEventHandler<UpdateUserDetailsEvent, bool>
    {
        private readonly DatabaseContext _context;
        private readonly UserContextService _userContextService;
        private readonly ILogger<UpdateUserDetailsEventHandler> _logger;

        public UpdateUserDetailsEventHandler(DatabaseContext context, UserContextService userContextService, ILogger<UpdateUserDetailsEventHandler> logger)
        {
            _context = context;
            _userContextService = userContextService;
            _logger = logger;
        }
        
        public async Task<bool> Handle(UpdateUserDetailsEvent @event, CancellationToken cancellationToken = default)
        {
            var currentUser = await _userContextService.GetUser();
            var existingUser = await _context.Users.SingleAsync(u => u.Id == currentUser.Id, cancellationToken);

            if (@event.Email != null)
            {
                existingUser.Email = @event.Email;
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("User details of {Username} updated", existingUser.Username);
            return true;
        }
    }
}