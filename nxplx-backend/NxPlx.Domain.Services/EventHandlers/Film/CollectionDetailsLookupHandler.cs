using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Film;
using NxPlx.Domain.Events.Film;
using NxPlx.Domain.Models.Database;
using NxPlx.Domain.Models.Details.Film;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Film
{
    public class CollectionDetailsLookupHandler : IDomainEventHandler<CollectionDetailsLookupQuery, MovieCollectionDto>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public CollectionDetailsLookupHandler(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }
        public async Task<MovieCollectionDto> Handle(CollectionDetailsLookupQuery query, CancellationToken cancellationToken = default)
        {
            var filmFiles = await _databaseContext.FilmFiles
                .Include(ff => ff.FilmDetails).ThenInclude(fd => fd.Genres)
                .Include(ff => ff.FilmDetails).ThenInclude(fd => fd.BelongsInCollection)
                .Where(ff => ff.FilmDetails.BelongsInCollectionId == query.CollectionId)
                .ToListAsync(cancellationToken);

            var collection = _mapper.Map<MovieCollection, MovieCollectionDto>(filmFiles.First().FilmDetails.BelongsInCollection);
            collection!.Movies = filmFiles.Select(ff => _mapper.Map<DbFilmDetails, OverviewElementDto>(ff.FilmDetails)).ToList();
            
            return collection;
        }
    }
}