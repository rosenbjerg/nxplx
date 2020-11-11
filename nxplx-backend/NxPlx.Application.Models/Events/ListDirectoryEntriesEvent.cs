using System.Collections.Generic;

namespace NxPlx.Application.Models.Events
{
    public class ListDirectoryEntriesEvent : IEvent<List<string>>
    {
        public ListDirectoryEntriesEvent(string currentWorkingDirectory)
        {
            CurrentWorkingDirectory = currentWorkingDirectory;
        }
        public string CurrentWorkingDirectory { get; }
    }
}