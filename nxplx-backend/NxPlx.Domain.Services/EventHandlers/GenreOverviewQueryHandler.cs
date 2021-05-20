using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models;
using NxPlx.Domain.Events;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers
{
    public class GenreOverviewQueryHandler : IDomainEventHandler<GenreOverviewQuery, IEnumerable<GenreDto>>
    {
        private readonly IMapper _mapper;
        private readonly ReadOnlyDatabaseContext _databaseContext;
        
        public GenreOverviewQueryHandler(
            IMapper mapper,
            ReadOnlyDatabaseContext databaseContext)
        {
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<GenreDto>> Handle(GenreOverviewQuery @event, CancellationToken cancellationToken = default)
        {
            return await _databaseContext.Genre
                .ProjectTo<GenreDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}