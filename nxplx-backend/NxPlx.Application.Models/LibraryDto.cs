namespace NxPlx.Application.Models
{
    public class LibraryDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Language { get; set; } = null!;
        public string Kind { get; set; } = null!;
    }
}