using System.Collections.Generic;

namespace NxPlx.Application.Models.Events.Library
{
    public class SetLibraryAccessCommand : ICommand<bool>
    {
        public SetLibraryAccessCommand(int userId, List<int> libraryIds)
        {
            UserId = userId;
            LibraryIds = libraryIds;
        }
        public int UserId { get; }
        public List<int> LibraryIds { get; }
    }
}