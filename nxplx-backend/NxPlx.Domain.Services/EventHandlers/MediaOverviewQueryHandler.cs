using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models;
using NxPlx.Domain.Events;
using NxPlx.Domain.Events.Library;
using NxPlx.Domain.Models.Database;
using NxPlx.Domain.Models.Details.Film;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Infrastructure.Events.Handling;
using IMapper = AutoMapper.IMapper;

namespace NxPlx.Domain.Services.EventHandlers
{
    public class MediaOverviewQueryHandler : IDomainEventHandler<MediaOverviewQuery, IEnumerable<OverviewElementDto>>
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IDomainEventDispatcher _dispatcher;

        public MediaOverviewQueryHandler(
            DatabaseContext context,
            IMapper mapper,
            IDomainEventDispatcher dispatcher)
        {
            _context = context;
            _mapper = mapper;
            _dispatcher = dispatcher;
        }
        

        public async Task<IEnumerable<OverviewElementDto>> Handle(MediaOverviewQuery @event, CancellationToken cancellationToken = default)
        {
            var libs = await _dispatcher.Dispatch(new CurrentUserLibraryAccessQuery());
            
            var seriesDetailIds = await _context.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId != null && libs.Contains(ef.PartOfLibraryId))
                .Select(ef => ef.SeriesDetailsId).Distinct()
                .ToListAsync(cancellationToken);
            var seriesDetails = await _context.SeriesDetails
                .Where(sd => seriesDetailIds.Contains(sd.Id))
                .ProjectTo<OverviewElementDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var filmDetailsIds = await _context.FilmFiles
                .Where(ff => ff.FilmDetailsId != null && libs.Contains(ff.PartOfLibraryId))
                .Select(ff => ff.FilmDetailsId)
                .ToListAsync(cancellationToken);
            var filmDetails = await _context.FilmDetails
                .AsNoTracking()
                .Where(fd => filmDetailsIds.Contains(fd.Id))
                .Include(ff => ff.Genres)
                .Include(ff => ff.BelongsInCollection)
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
            overview.AddRange(_mapper.Map<List<DbFilmDetails>, List<OverviewElementDto>>(notInCollections));
            overview.AddRange(_mapper.Map<List<MovieCollection>, List<OverviewElementDto>>(collections));

            return overview.OrderBy(oe => oe.Title).ToList();
        }
    }
}