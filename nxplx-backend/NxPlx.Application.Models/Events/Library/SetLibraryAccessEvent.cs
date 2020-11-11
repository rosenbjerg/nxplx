using System.Collections.Generic;

namespace NxPlx.Application.Models.Events
{
    public class SetLibraryAccessEvent : IEvent<bool>
    {
        public SetLibraryAccessEvent(int userId, List<int> libraryIds)
        {
            UserId = userId;
            LibraryIds = libraryIds;
        }
        public int UserId { get; }
        public List<int> LibraryIds { get; }
    }
}