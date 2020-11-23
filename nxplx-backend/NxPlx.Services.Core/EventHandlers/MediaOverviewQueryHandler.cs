using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Film;
using IMapper = AutoMapper.IMapper;

namespace NxPlx.Core.Services.EventHandlers
{
    public class MediaOverviewQueryHandler : CachedResultEventHandlerBase<MediaOverviewQuery, IEnumerable<OverviewElementDto>>
    {
        private readonly DatabaseContext _context;
        private readonly IDtoMapper _dtoMapper;
        private readonly IMapper _mapper;
        private readonly IEventDispatcher _dispatcher;

        public MediaOverviewQueryHandler(IDistributedCache distributedCache, DatabaseContext context, IDtoMapper dtoMapper, IMapper mapper, IEventDispatcher dispatcher) : base(distributedCache)
        {
            _context = context;
            _dtoMapper = dtoMapper;
            _mapper = mapper;
            _dispatcher = dispatcher;
        }

        protected override async Task<(string CacheKey, CacheResultGenerator<MediaOverviewQuery, IEnumerable<OverviewElementDto>> cacheGenerator)> Prepare(MediaOverviewQuery @event, CancellationToken cancellationToken = default)
        {
            var currentUser = await _dispatcher.Dispatch<Models.User>(new CurrentUserQuery());
            var libs = currentUser.LibraryAccessIds;
            if (currentUser.Admin) libs = await _context.Libraries.Select(l => l.Id).ToListAsync(cancellationToken);
            var overviewCacheKey = "OVERVIEW:" + string.Join(',', libs.OrderBy(i => i));
            return (overviewCacheKey, (query, cancellation) => BuildOverview(libs, cancellation));
        }
        
        private async Task<IEnumerable<OverviewElementDto>> BuildOverview(List<int> libs, CancellationToken cancellationToken)
        {
            var seriesDetailIds = await _context.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId != null && libs.Contains(ef.PartOfLibraryId))
                .Select(ef => ef.SeriesDetailsId).Distinct()
                .ToListAsync(cancellationToken);

            var seriesDetails = await _context.SeriesDetails
                .Where(sd => seriesDetailIds.Contains(sd.Id))
                .Project<DbSeriesDetails, OverviewElementDto>(_dtoMapper)
                .ToListAsync(cancellationToken);
            
            var filmDetails = await _context.FilmFiles
                .Where(ff => ff.FilmDetailsId != null && libs.Contains(ff.PartOfLibraryId))
                .Select(ff => ff.FilmDetails).Distinct()
                .ToListAsync(cancellationToken);
            
            var collections = filmDetails
                .Where(fd => fd.BelongsInCollectionId.HasValue)
                .GroupBy(fd => fd.BelongsInCollectionId)
                .Where(group => group.Count() > 1)
                .Select(group => group.First().BelongsInCollection)
                .ToList();

            var collectionIds = collections.Select(c => c.Id).ToList();
            var notInCollections = filmDetails.Where(fd => fd.BelongsInCollectionId == null || !collectionIds.Contains(fd.BelongsInCollectionId.Value)).ToList();

            var overview = new List<OverviewElementDto>(seriesDetails.Count + notInCollections.Count + collections.Count);
            overview.AddRange(seriesDetails);
            overview.AddRange(_mapper.Map<List<DbFilmDetails>, IEnumerable<OverviewElementDto>>(notInCollections));
            overview.AddRange(_mapper.Map<List<MovieCollection>, IEnumerable<OverviewElementDto>>(collections));

            return overview.OrderBy(oe => oe.Title).ToList();
        }
    }
}