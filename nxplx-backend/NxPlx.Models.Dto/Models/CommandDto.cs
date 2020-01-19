namespace NxPlx.Models.Dto.Models
{
    public class CommandDto : IDto
    {
        public string name { get; set; }
        public string description { get; set; }
        public string[] arguments { get; set; }
    }
}