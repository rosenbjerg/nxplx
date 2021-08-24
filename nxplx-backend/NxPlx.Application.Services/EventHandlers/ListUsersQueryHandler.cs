using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Application.Events;
using NxPlx.Application.Models;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers
{
    public class ListUsersQueryHandler : IApplicationEventHandler<ListUsersQuery, IEnumerable<UserDto>>
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly ConnectionHub _connectionHub;
        private readonly IOperationContext _operationContext;

        public ListUsersQueryHandler(DatabaseContext context, IMapper mapper, ConnectionHub connectionHub, IOperationContext operationContext)
        {
            _context = context;
            _mapper = mapper;
            _connectionHub = connectionHub;
            _operationContext = operationContext;
        }

        public async Task<IEnumerable<UserDto>> Handle(ListUsersQuery query, CancellationToken cancellationToken = default)
        {
            var users = await _context.Users.AsNoTracking()
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var online = _connectionHub.ConnectedIds();
            foreach (var user in users.Where(u => u.Id == _operationContext.Session.UserId || online.Contains(u.Id)))
            {
                user.IsOnline = true;
            }

            return users;
        }
    }
}