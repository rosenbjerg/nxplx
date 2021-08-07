using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Events;
using NxPlx.Domain.Models;
using NxPlx.Domain.Services;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers
{
    public class CreateAdminCommandHandler : IApplicationEventHandler<CreateAdminCommand, bool>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<CreateAdminCommandHandler> _logger;

        public CreateAdminCommandHandler(DatabaseContext databaseContext, ILogger<CreateAdminCommandHandler> logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }

        public async Task<bool> Handle(CreateAdminCommand command, CancellationToken cancellationToken = default)
        {
            if (await _databaseContext.Users.AnyAsync(u => u.Username == "admin", cancellationToken))
                return false;
            
            _databaseContext.Add(new User
            {
                Username = "admin",
                PasswordHash = PasswordUtils.Hash("changemebaby"),
                Admin = true
            });
            await _databaseContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}