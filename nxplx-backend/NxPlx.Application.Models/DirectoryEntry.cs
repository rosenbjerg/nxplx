using System.Collections.Generic;

namespace NxPlx.Application.Models
{
    public record DirectoryEntries(string Parent, string Current, List<DirectoryEntry> Directories);
    public record DirectoryEntry(string Name, string Path);
}