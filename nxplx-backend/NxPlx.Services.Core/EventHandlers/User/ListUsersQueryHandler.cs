using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.Film;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.User
{
    public class ListUsersQueryHandler : IEventHandler<ListUsersQuery, IEnumerable<UserDto>>
    {
        private readonly DatabaseContext _context;
        private readonly IDtoMapper _dtoMapper;

        public ListUsersQueryHandler(DatabaseContext context, IDtoMapper dtoMapper)
        {
            _context = context;
            _dtoMapper = dtoMapper;
        }

        public async Task<IEnumerable<UserDto>> Handle(ListUsersQuery query, CancellationToken cancellationToken = default)
        {
            var users = await _context.Users.AsNoTracking().ToListAsync(cancellationToken);
            return _dtoMapper.Map<Models.User, UserDto>(users);
        }
    }
}