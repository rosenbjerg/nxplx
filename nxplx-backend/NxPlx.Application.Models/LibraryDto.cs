namespace NxPlx.Application.Models
{
    public class LibraryDto : IDto
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string language { get; set; } = null!;
        public string kind { get; set; } = null!;
    }
    public class AdminLibraryDto : LibraryDto
    {
        public string path { get; set; } = null!;
    }
}