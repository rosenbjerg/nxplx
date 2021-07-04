namespace NxPlx.Domain.Models.File
{
    public interface ILibraryMember
    {
        Library PartOfLibrary { get; }
        
        int PartOfLibraryId { get; }
    }
}