using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Application.Models.Film;
using NxPlx.Infrastructure.Database;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Film;

namespace NxPlx.Core.Services.EventHandlers.Film
{
    public class CollectionDetailsLookupHandler : IEventHandler<CollectionDetailsLookupEvent, MovieCollectionDto>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IDtoMapper _dtoMapper;

        public CollectionDetailsLookupHandler(DatabaseContext databaseContext, IDtoMapper dtoMapper)
        {
            _databaseContext = databaseContext;
            _dtoMapper = dtoMapper;
        }
        public async Task<MovieCollectionDto> Handle(CollectionDetailsLookupEvent @event, CancellationToken cancellationToken = default)
        {
            var filmFiles = await _databaseContext.FilmFiles
                .Where(ff => ff.FilmDetails.BelongsInCollectionId == @event.CollectionId)
                .ToListAsync(cancellationToken);

            var collection = _dtoMapper.Map<MovieCollection, MovieCollectionDto>(filmFiles.First().FilmDetails.BelongsInCollection);
            collection!.Movies = _dtoMapper.Map<DbFilmDetails, OverviewElementDto>(filmFiles.Select(ff => ff.FilmDetails)).ToList();
            
            return collection;
        }
    }
}