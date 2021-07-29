using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models;
using NxPlx.Domain.Events.File;
using NxPlx.Domain.Events.Film;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Infrastructure.Events.Handling;
using IMapper = AutoMapper.IMapper;

namespace NxPlx.Domain.Services.EventHandlers.Film
{
    public class FilmFileInfoLookupHandler : IDomainEventHandler<FilmInfoLookupQuery, InfoDto>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;

        public FilmFileInfoLookupHandler(DatabaseContext databaseContext, IDomainEventDispatcher eventDispatcher, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
        }
        
        public async Task<InfoDto> Handle(FilmInfoLookupQuery query, CancellationToken cancellationToken = default)
        {
            var dto = await _databaseContext.FilmFiles
                .Where(ff => ff.Id == query.FileId)
                .ProjectTo<InfoDto>(_mapper.ConfigurationProvider)
                .SingleAsync(cancellationToken);

            dto.FilePath = await _eventDispatcher.Dispatch(new StreamUrlQuery(StreamKind.Film, dto.Id));
            return dto;
        }
    }
}