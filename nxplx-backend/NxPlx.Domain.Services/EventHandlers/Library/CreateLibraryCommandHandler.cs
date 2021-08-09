using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Jobs;
using NxPlx.Domain.Events.Library;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Library
{
    public class CreateLibraryCommandHandler : IDomainEventHandler<CreateLibraryCommand, AdminLibraryDto>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<CreateLibraryCommandHandler> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;

        public CreateLibraryCommandHandler(DatabaseContext context, ILogger<CreateLibraryCommandHandler> logger, IDistributedCache distributedCache, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _distributedCache = distributedCache;
            _mapper = mapper;
        }
        public async Task<AdminLibraryDto> Handle(CreateLibraryCommand command, CancellationToken cancellationToken = default)
        {
            var lib = new Domain.Models.Library
            {
                Name = command.Name,
                Path = command.Path,
                Language = command.Language,
                Kind = command.Kind == "film" ? LibraryKind.Film : LibraryKind.Series
            };

            _context.Libraries.Add(lib);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created library {Name} with {Path}", lib.Name, lib.Path);
            await _distributedCache.ClearList("overview", "sys", cancellationToken);
            
            return _mapper.Map<Domain.Models.Library, AdminLibraryDto>(lib)!;
        }
    }
}