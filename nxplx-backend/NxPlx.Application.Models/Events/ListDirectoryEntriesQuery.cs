using System.Collections.Generic;

namespace NxPlx.Application.Models.Events
{
    public class ListDirectoryEntriesQuery : IQuery<List<string>>
    {
        public ListDirectoryEntriesQuery(string currentWorkingDirectory)
        {
            CurrentWorkingDirectory = currentWorkingDirectory;
        }
        public string CurrentWorkingDirectory { get; }
    }
}