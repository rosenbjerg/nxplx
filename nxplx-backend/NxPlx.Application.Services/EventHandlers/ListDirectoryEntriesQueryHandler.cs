using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers
{
    public class ListDirectoryEntriesQueryHandler : IApplicationEventHandler<ListDirectoryEntriesQuery, List<string>>
    {
        public Task<List<string>> Handle(ListDirectoryEntriesQuery query, CancellationToken cancellationToken = default)
        {
            if (query.CurrentWorkingDirectory == string.Empty || !Directory.Exists(query.CurrentWorkingDirectory))
                return Task.FromResult(new List<string>());

            return Task.FromResult(Directory.EnumerateDirectories(query.CurrentWorkingDirectory.Replace("\\", "/"), "*", new EnumerationOptions
            {
                AttributesToSkip = FileAttributes.Hidden | FileAttributes.Temporary | FileAttributes.System
            }).Select(Path.GetFileName).ToList())!;
        }
    }
}