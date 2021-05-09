using System.Collections.Generic;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Library
{
    public class SetLibraryAccessCommand : IDomainCommand<bool>
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