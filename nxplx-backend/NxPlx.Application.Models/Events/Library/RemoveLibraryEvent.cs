namespace NxPlx.Application.Models.Events.Library
{
    public class RemoveLibraryCommand : ICommand<bool>
    {
        public RemoveLibraryCommand(int libraryId)
        {
            LibraryId = libraryId;
        }
        public int LibraryId { get; }
    }
}