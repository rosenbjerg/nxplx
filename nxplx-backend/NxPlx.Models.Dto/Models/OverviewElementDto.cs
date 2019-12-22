namespace NxPlx.Models.Dto.Models
{
    public class OverviewElementDto : IDto
    {
        public int id { get; set; }
        public string title { get; set; }
        public string poster { get; set; }
        public string kind { get; set; }
    }
}