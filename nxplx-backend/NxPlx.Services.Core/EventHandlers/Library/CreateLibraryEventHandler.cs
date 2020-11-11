using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;
using NxPlx.Models;

namespace NxPlx.Core.Services.EventHandlers.Library
{
    public class CreateLibraryEventHandler : IEventHandler<CreateLibraryEvent, AdminLibraryDto>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<CreateLibraryEventHandler> _logger;
        private readonly ICacheClearer _cacheClearer;
        private readonly IDtoMapper _dtoMapper;

        public CreateLibraryEventHandler(DatabaseContext context, ILogger<CreateLibraryEventHandler> logger, ICacheClearer cacheClearer, IDtoMapper dtoMapper)
        {
            _context = context;
            _logger = logger;
            _cacheClearer = cacheClearer;
            _dtoMapper = dtoMapper;
        }
        public async Task<AdminLibraryDto> Handle(CreateLibraryEvent @event, CancellationToken cancellationToken = default)
        {
            var lib = new Models.Library
            {
                Name = @event.Name,
                Path = @event.Path,
                Language = @event.Language,
                Kind = @event.Kind == "film" ? LibraryKind.Film : LibraryKind.Series
            };

            _context.Libraries.Add(lib);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created library {Name} with {Path}", lib.Name, lib.Path);
            await _cacheClearer.Clear("OVERVIEW");
            
            return _dtoMapper.Map<Models.Library, AdminLibraryDto>(lib)!;
        }
    }
}