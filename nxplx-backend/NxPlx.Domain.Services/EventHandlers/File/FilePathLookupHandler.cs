using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Application.Models;
using NxPlx.Domain.Events.File;
using NxPlx.Domain.Events.Library;
using NxPlx.Domain.Models.File;
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
}