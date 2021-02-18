using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models.Film;
using NxPlx.Domain.Events.Film;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Film
{
    public class FilmDetailsLookupHandler : IDomainEventHandler<FilmDetailsLookupQuery, FilmDto?>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public FilmDetailsLookupHandler(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }
        public async Task<FilmDto?> Handle(FilmDetailsLookupQuery query, CancellationToken cancellationToken = default)
        {
            return await _databaseContext.FilmFiles
                .Where(ff => ff.FilmDetailsId == query.FileDetailsId)
                .ProjectTo<FilmDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}