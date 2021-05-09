using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models;
using NxPlx.Domain.Events.Film;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.User
{
    public class ListUsersQueryHandler : IDomainEventHandler<ListUsersQuery, IEnumerable<UserDto>>
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public ListUsersQueryHandler(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> Handle(ListUsersQuery query, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AsNoTracking()
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}