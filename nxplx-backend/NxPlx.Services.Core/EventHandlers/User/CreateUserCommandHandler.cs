using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.Film;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.User
{
    public class CreateUserCommandHandler : IEventHandler<CreateUserCommand, UserDto>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IDtoMapper _dtoMapper;

        public CreateUserCommandHandler(DatabaseContext context, ILogger<CreateUserCommandHandler> logger, IDtoMapper dtoMapper)
        {
            _context = context;
            _logger = logger;
            _dtoMapper = dtoMapper;
        }
        
        public async Task<UserDto> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
        {
            var user = new Models.User
            {
                Username = command.Username,
                Email = command.Email,
                Admin = command.IsAdmin,
                LibraryAccessIds = command.LibraryIds ?? new List<int>(),
                PasswordHash = PasswordUtils.Hash(command.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created user {Username}", user.Username);
            return _dtoMapper.Map<Models.User, UserDto>(user)!;
        }
    }
}