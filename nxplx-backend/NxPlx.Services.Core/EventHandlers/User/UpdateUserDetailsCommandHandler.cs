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
    public class UpdateUserDetailsCommandHandler : IEventHandler<UpdateUserDetailsCommand, bool>
    {
        private readonly DatabaseContext _context;
        private readonly IEventDispatcher _dispatcher;
        private readonly ILogger<UpdateUserDetailsCommandHandler> _logger;

        public UpdateUserDetailsCommandHandler(DatabaseContext context, IEventDispatcher dispatcher, ILogger<UpdateUserDetailsCommandHandler> logger)
        {
            _context = context;
            _dispatcher = dispatcher;
            _logger = logger;
        }
        
        public async Task<bool> Handle(UpdateUserDetailsCommand command, CancellationToken cancellationToken = default)
        {
            var currentUser = await _dispatcher.Dispatch(new CurrentUserQuery());
            var existingUser = await _context.Users.SingleAsync(u => u.Id == currentUser.Id, cancellationToken);

            if (command.Email != null)
            {
                existingUser.Email = command.Email;
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("User details of {Username} updated", existingUser.Username);
            return true;
        }
    }
}