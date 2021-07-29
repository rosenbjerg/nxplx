using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Application.Events
{
    public class ListDirectoryEntriesQuery : IApplicationQuery<DirectoryEntries>
    {
        public ListDirectoryEntriesQuery(string currentWorkingDirectory)
        {
            CurrentWorkingDirectory = currentWorkingDirectory;
        }
        public string CurrentWorkingDirectory { get; }
    }
}