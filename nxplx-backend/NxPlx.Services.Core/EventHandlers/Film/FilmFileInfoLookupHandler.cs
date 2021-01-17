using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.File;
using NxPlx.Application.Models.Events.Film;
using NxPlx.Infrastructure.Database;
using NxPlx.Models.File;
using IMapper = AutoMapper.IMapper;

namespace NxPlx.Core.Services.EventHandlers.Film
{
    public class FilmFileInfoLookupHandler : IEventHandler<FilmInfoLookupQuery, InfoDto?>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IDtoMapper _dtoMapper;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;

        public FilmFileInfoLookupHandler(DatabaseContext databaseContext, IDtoMapper dtoMapper, IEventDispatcher eventDispatcher, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _dtoMapper = dtoMapper;
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
        }
        
        public async Task<InfoDto?> Handle(FilmInfoLookupQuery query, CancellationToken cancellationToken = default)
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