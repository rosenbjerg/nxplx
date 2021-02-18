using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models;
using NxPlx.Domain.Events.Library;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Library
{
    public class ListLibrariesQueryHandler : IDomainEventHandler<ListLibrariesQuery, List<LibraryDto>>
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public ListLibrariesQueryHandler(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Task<List<LibraryDto>> Handle(ListLibrariesQuery query, CancellationToken cancellationToken = default)
        {
            return _context.Libraries.ProjectTo<LibraryDto>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}