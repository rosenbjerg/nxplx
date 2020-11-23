using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;
using NxPlx.Models.Details;

namespace NxPlx.Core.Services.EventHandlers
{
    public class GenreOverviewQueryHandler : CachedResultEventHandlerBase<GenreOverviewQuery, IEnumerable<GenreDto>>
    {
        private readonly DatabaseContext _context;
        private readonly IDtoMapper _dtoMapper;

        public GenreOverviewQueryHandler(IDistributedCache distributedCache, DatabaseContext context, IDtoMapper dtoMapper) : base(distributedCache)
        {
            _context = context;
            _dtoMapper = dtoMapper;
        }

        protected override async Task<(string CacheKey, CacheResultGenerator<GenreOverviewQuery, IEnumerable<GenreDto>> cacheGenerator)> Prepare(GenreOverviewQuery @event, CancellationToken cancellationToken = default)
        {
            return ("OVERVIEW:GENRES", async (query, cancellation) =>
            {
                var genres = await _context.Genre.AsNoTracking().ToListAsync(cancellation);
                return _dtoMapper.Map<Genre, GenreDto>(genres);
            });
        }
    }
}