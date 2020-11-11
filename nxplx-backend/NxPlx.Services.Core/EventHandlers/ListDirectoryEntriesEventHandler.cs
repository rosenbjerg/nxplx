using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Models.Events;

namespace NxPlx.Core.Services.EventHandlers
{
    public class ListDirectoryEntriesEventHandler : IEventHandler<ListDirectoryEntriesEvent, List<string>>
    {
        public Task<List<string>> Handle(ListDirectoryEntriesEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event.CurrentWorkingDirectory == string.Empty || !Directory.Exists(@event.CurrentWorkingDirectory))
                return Task.FromResult(new List<string>());

            return Task.FromResult(Directory.EnumerateDirectories(@event.CurrentWorkingDirectory.Replace("\\", "/"), "*", new EnumerationOptions
            {
                AttributesToSkip = FileAttributes.Hidden | FileAttributes.Temporary | FileAttributes.System
            }).Select(Path.GetFileName).ToList())!;
        }
    }
}