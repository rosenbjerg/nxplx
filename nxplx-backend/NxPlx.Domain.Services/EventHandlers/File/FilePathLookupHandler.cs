using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Abstractions;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Domain.Events.File;
using NxPlx.Domain.Events.Library;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.File
{
    public class FilePathLookupHandler : IDomainEventHandler<FilePathLookupQuery, string?>
    {
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IOperationContext _operationContext;
        private readonly DatabaseContext _databaseContext;

        public FilePathLookupHandler(IDomainEventDispatcher eventDispatcher, IOperationContext operationContext, DatabaseContext databaseContext)
        {
            _eventDispatcher = eventDispatcher;
            _operationContext = operationContext;
            _databaseContext = databaseContext;
        }
        public async Task<string?> Handle(FilePathLookupQuery query, CancellationToken cancellationToken = default)
        {
            var libraries = await _eventDispatcher.Dispatch(new LibraryAccessQuery(_operationContext.Session.UserId));

            return query.StreamKind switch
            {
                StreamKind.Episode => await _databaseContext.EpisodeFiles
                    .Where(e => libraries.Contains(e.PartOfLibraryId) && e.Id == query.Id).Select(e => e.Path)
                    .SingleAsync(cancellationToken),
                
                StreamKind.Film => await _databaseContext.FilmFiles
                    .Where(f => libraries.Contains(f.PartOfLibraryId) && f.Id == query.Id).Select(f => f.Path)
                    .SingleAsync(cancellationToken),
                
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }    
    public class FileReservationCommandHandler : IDomainEventHandler<FileReservationCommand, bool>
    {
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IOperationContext _operationContext;
        private readonly DatabaseContext _databaseContext;
        private readonly IDistributedCache _distributedCache;

        public FileReservationCommandHandler(IDomainEventDispatcher eventDispatcher, IOperationContext operationContext, DatabaseContext databaseContext, IDistributedCache distributedCache)
        {
            _eventDispatcher = eventDispatcher;
            _operationContext = operationContext;
            _databaseContext = databaseContext;
            _distributedCache = distributedCache;
        }
        public async Task<bool> Handle(FileReservationCommand query, CancellationToken cancellationToken = default)
        {
            var key = $"{query.StreamKind}={query.Id}";
            var userId = _operationContext.Session.UserId;
            await _distributedCache.AddToList("reservation", "sys", key, userId, TimeSpan.FromMinutes(5), cancellationToken);
            await _distributedCache.SetStringAsync($"reservation:{userId}:{key}", "", new DistributedCacheEntryOptions{ AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) }, cancellationToken);
            return true;

        }
    }

    public class FileReservationService
    {
        
    }

    public class DA
    {
        
    }
}