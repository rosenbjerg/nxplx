namespace NxPlx.Application.Models.Film
{
    public class ProductionCompanyDto : IDto
    {
        public int id { get; set; }
        public string logo { get; set; } = null!;
        public string name { get; set; } = null!;
        public string originCountry { get; set; } = null!;
    }
}