namespace NxPlx.Domain.Models
{
    public class Library : EntityBase
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Language { get; set; }
        public LibraryKind Kind { get; set; }
    }
}