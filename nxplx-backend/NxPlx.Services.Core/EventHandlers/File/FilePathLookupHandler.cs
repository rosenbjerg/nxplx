using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.File;
using NxPlx.Application.Models.Events.Library;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.File
{
    public class FilePathLookupHandler : IEventHandler<FilePathLookupQuery, string?>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IOperationContext _operationContext;
        private readonly DatabaseContext _databaseContext;

        public FilePathLookupHandler(IEventDispatcher eventDispatcher, IOperationContext operationContext, DatabaseContext databaseContext)
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