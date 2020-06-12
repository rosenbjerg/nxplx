namespace NxPlx.Application.Models
{
    public class ProductionCountryDto : IDto
    {
        public string iso3166_1 { get; set; } = null!;
        public string name { get; set; } = null!;
    }
}