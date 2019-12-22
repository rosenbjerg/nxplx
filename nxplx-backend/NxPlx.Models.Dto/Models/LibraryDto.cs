namespace NxPlx.Models.Dto.Models
{
    public class LibraryDto : IDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string language { get; set; }
        public string kind { get; set; }
    }
    public class AdminLibraryDto : LibraryDto
    {
        public string path { get; set; }
    }
}