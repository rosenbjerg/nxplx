namespace NxPlx.Application.Models.Series
{
    public class NetworkDto : IDto
    {
        public string name { get; set; } = null!;
        public string logo { get; set; } = null!;
        public string originCountry { get; set; } = null!;
    }
}