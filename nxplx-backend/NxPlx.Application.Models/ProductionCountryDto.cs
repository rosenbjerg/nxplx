namespace NxPlx.Application.Models
{
    public class ProductionCountryDto : IDto
    {
        public string Iso31661 { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}