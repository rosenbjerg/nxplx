using System.Collections.Generic;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Models.Events
{
    public class ListDirectoryEntriesQuery : IApplicationQuery<List<string>>
    {
        public ListDirectoryEntriesQuery(string currentWorkingDirectory)
        {
            CurrentWorkingDirectory = currentWorkingDirectory;
        }
        public string CurrentWorkingDirectory { get; }
    }
}