using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Library
{
    public class RemoveLibraryCommand : IDomainCommand<bool>
    {
        public RemoveLibraryCommand(int libraryId)
        {
            LibraryId = libraryId;
        }
        public int LibraryId { get; }
    }
}