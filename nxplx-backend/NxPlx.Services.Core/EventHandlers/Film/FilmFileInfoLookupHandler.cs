using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.File;
using NxPlx.Application.Models.Events.Film;
using NxPlx.Infrastructure.Database;
using NxPlx.Models.File;

namespace NxPlx.Core.Services.EventHandlers.Film
{
    public class FilmFileInfoLookupHandler : IEventHandler<FilmInfoLookupQuery, InfoDto>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IDtoMapper _dtoMapper;
        private readonly IEventDispatcher _eventDispatcher;

        public FilmFileInfoLookupHandler(DatabaseContext databaseContext, IDtoMapper dtoMapper, IEventDispatcher eventDispatcher)
        {
            _databaseContext = databaseContext;
            _dtoMapper = dtoMapper;
            _eventDispatcher = eventDispatcher;
        }
        
        public async Task<InfoDto> Handle(FilmInfoLookupQuery query, CancellationToken cancellationToken = default)
        {
            var filmFile = await _databaseContext.FilmFiles.FirstOrDefaultAsync(ff => ff.Id == query.FileId, cancellationToken);
            var dto = _dtoMapper.Map<FilmFile, InfoDto>(filmFile);
            dto!.FileToken = await _eventDispatcher.Dispatch(new RequestFileTokenCommand(filmFile.Path));
            return dto;
        }
    }
}