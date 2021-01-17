using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.File;
using NxPlx.Application.Models.Events.Series;
using NxPlx.Infrastructure.Database;
using NxPlx.Models.File;

namespace NxPlx.Core.Services.EventHandlers.Series
{
    public class EpisodeFileInfoQueryHandler : IEventHandler<EpisodeFileInfoQuery, InfoDto?>
    {
        private readonly DatabaseContext _context;
        private readonly IDtoMapper _dtoMapper;
        private readonly IEventDispatcher _dispatcher;

        public EpisodeFileInfoQueryHandler(DatabaseContext context, IDtoMapper dtoMapper, IEventDispatcher dispatcher)
        {
            _context = context;
            _dtoMapper = dtoMapper;
            _dispatcher = dispatcher;
        }
        public async Task<InfoDto?> Handle(EpisodeFileInfoQuery @event, CancellationToken cancellationToken = default)
        {
            var episodeFile = await _context.EpisodeFiles
                .Include(ef => ef.SeriesDetails)
                .ThenInclude(sd => sd.Seasons)
                .ThenInclude(sd => sd.Episodes)
                .Include(ef => ef.Subtitles)
                .Where(ef => ef.Id == @event.Id)
                .FirstOrDefaultAsync(cancellationToken);
            var dto = _dtoMapper.Map<EpisodeFile, InfoDto>(episodeFile);
            dto!.FilePath = await _dispatcher.Dispatch(new StreamUrlQuery(StreamKind.Episode, episodeFile.Id));
            return dto;
        }
    }
}