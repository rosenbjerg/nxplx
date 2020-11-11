namespace NxPlx.Application.Models.Events
{
    public class RemoveLibraryEvent : IEvent<bool>
    {
        public RemoveLibraryEvent(int libraryId)
        {
            LibraryId = libraryId;
        }
        public int LibraryId { get; }
    }
}