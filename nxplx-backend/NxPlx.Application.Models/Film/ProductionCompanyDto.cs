namespace NxPlx.Application.Models.Film
{
    public class ProductionCompanyDto : IDto
    {
        public int Id { get; set; }
        public string Logo { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string OriginCountry { get; set; } = null!;
    }
}