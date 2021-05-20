using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Models;
using NxPlx.Domain.Events.Film;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.User
{
    public class CreateUserCommandHandler : IDomainEventHandler<CreateUserCommand, UserDto>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(DatabaseContext context, ILogger<CreateUserCommandHandler> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        
        public async Task<UserDto> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
        {
            var user = new Domain.Models.User
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
            return _mapper.Map<Domain.Models.User, UserDto>(user)!;
        }
    }
}