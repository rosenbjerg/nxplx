using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Events;
using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers
{
    public class ListDirectoryEntriesQueryHandler : IApplicationEventHandler<ListDirectoryEntriesQuery, DirectoryEntries>
    {
        private readonly EnumerationOptions _enumerationOptions = new EnumerationOptions
        {
            AttributesToSkip = FileAttributes.Hidden | FileAttributes.Temporary | FileAttributes.System
        };
        
        public Task<DirectoryEntries> Handle(ListDirectoryEntriesQuery query, CancellationToken cancellationToken = default)
        {
            var result = HandleInternal(query);
            return Task.FromResult(result);
        }
        private DirectoryEntries HandleInternal(ListDirectoryEntriesQuery query)
        {
            if (query.CurrentWorkingDirectory == string.Empty)
            {
                var directories = DriveInfo.GetDrives()
                    .Select(s => new DirectoryEntry(s.RootDirectory.FullName, s.RootDirectory.FullName))
                    .ToList();
                return new DirectoryEntries(string.Empty, string.Empty, directories);
            }
            else
            {
                if (!Directory.Exists(query.CurrentWorkingDirectory))
                    return new DirectoryEntries(String.Empty, query.CurrentWorkingDirectory, new List<DirectoryEntry>());

                
                var cleanedName = query.CurrentWorkingDirectory.Replace("\\", "/");
                var directories = Directory
                    .EnumerateDirectories(cleanedName, "*", _enumerationOptions)
                    .Select(dir => new DirectoryEntry(Path.GetFileName(dir), dir))
                    .ToList()!;
                return new DirectoryEntries(Path.GetDirectoryName(cleanedName) ?? string.Empty, cleanedName, directories);
            }
        }
    }
}