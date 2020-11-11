using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.Library
{
    public class ListAdminLibrariesEventHandler : IEventHandler<ListAdminLibrariesEvent, List<AdminLibraryDto>>
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public ListAdminLibrariesEventHandler(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Task<List<AdminLibraryDto>> Handle(ListAdminLibrariesEvent @event, CancellationToken cancellationToken = default)
        {
            return _context.Libraries.ProjectTo<AdminLibraryDto>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}