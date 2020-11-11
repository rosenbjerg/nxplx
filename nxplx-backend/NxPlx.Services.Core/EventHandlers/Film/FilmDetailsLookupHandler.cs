using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models.Events;
using NxPlx.Application.Models.Film;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.Film
{
    public class FilmDetailsLookupHandler : IEventHandler<FilmDetailsLookupEvent, FilmDto>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public FilmDetailsLookupHandler(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }
        public async Task<FilmDto?> Handle(FilmDetailsLookupEvent @event, CancellationToken cancellationToken = default)
        {
            return await _databaseContext.FilmFiles
                .Where(ff => ff.FilmDetailsId == @event.FileDetailsId)
                .ProjectTo<FilmDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}