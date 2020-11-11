using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.User
{
    public class CreateUserEventHandler : IEventHandler<CreateUserEvent, UserDto>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<CreateUserEventHandler> _logger;
        private readonly IDtoMapper _dtoMapper;

        public CreateUserEventHandler(DatabaseContext context, ILogger<CreateUserEventHandler> logger, IDtoMapper dtoMapper)
        {
            _context = context;
            _logger = logger;
            _dtoMapper = dtoMapper;
        }
        
        public async Task<UserDto> Handle(CreateUserEvent @event, CancellationToken cancellationToken = default)
        {
            var user = new Models.User
            {
                Username = @event.Username,
                Email = @event.Email,
                Admin = @event.IsAdmin,
                LibraryAccessIds = @event.LibraryIds ?? new List<int>(),
                PasswordHash = PasswordUtils.Hash(@event.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created user {Username}", user.Username);
            return _dtoMapper.Map<Models.User, UserDto>(user)!;
        }
    }
}