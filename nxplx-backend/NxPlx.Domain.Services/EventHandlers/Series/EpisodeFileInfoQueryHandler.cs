using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models;
using NxPlx.Domain.Events.File;
using NxPlx.Domain.Events.Series;
using NxPlx.Domain.Models.File;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Series
{
    public class EpisodeFileInfoQueryHandler : IDomainEventHandler<EpisodeFileInfoQuery, EpisodeInfoDto?>
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IDomainEventDispatcher _dispatcher;

        public EpisodeFileInfoQueryHandler(DatabaseContext context, IMapper mapper, IDomainEventDispatcher dispatcher)
        {
            _context = context;
            _mapper = mapper;
            _dispatcher = dispatcher;
        }
        public async Task<EpisodeInfoDto?> Handle(EpisodeFileInfoQuery @event, CancellationToken cancellationToken = default)
        {
            var episodeFile = await _context.EpisodeFiles
                .Include(ef => ef.SeriesDetails)
                .ThenInclude(sd => sd.Seasons)
                .ThenInclude(sd => sd.Episodes)
                .Include(ef => ef.Subtitles)
                .Where(ef => ef.Id == @event.Id)
                .FirstOrDefaultAsync(cancellationToken);
            var dto = _mapper.Map<EpisodeFile, EpisodeInfoDto>(episodeFile);
            dto!.FilePath = await _dispatcher.Dispatch(new StreamUrlQuery(StreamKind.Episode, episodeFile.Id));
            return dto;
        }
    }
}