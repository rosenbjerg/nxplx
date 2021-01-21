using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.Library;
using NxPlx.Infrastructure.Database;
using NxPlx.Models;

namespace NxPlx.Core.Services.EventHandlers.Library
{
    public class CreateLibraryCommandHandler : IEventHandler<CreateLibraryCommand, AdminLibraryDto>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<CreateLibraryCommandHandler> _logger;
        private readonly ICacheClearer _cacheClearer;
        private readonly IDtoMapper _dtoMapper;

        public CreateLibraryCommandHandler(DatabaseContext context, ILogger<CreateLibraryCommandHandler> logger, ICacheClearer cacheClearer, IDtoMapper dtoMapper)
        {
            _context = context;
            _logger = logger;
            _cacheClearer = cacheClearer;
            _dtoMapper = dtoMapper;
        }
        public async Task<AdminLibraryDto> Handle(CreateLibraryCommand command, CancellationToken cancellationToken = default)
        {
            var lib = new Models.Library
            {
                Name = command.Name,
                Path = command.Path,
                Language = command.Language,
                Kind = command.Kind == "film" ? LibraryKind.Film : LibraryKind.Series
            };

            _context.Libraries.Add(lib);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created library {Name} with {Path}", lib.Name, lib.Path);
            await _cacheClearer.Clear("OVERVIEW");
            
            return _dtoMapper.Map<Models.Library, AdminLibraryDto>(lib)!;
        }
    }
}