namespace NxPlx.Models.Dto.Models.Film
{
    public class ProductionCompanyDto : IDto
    {
        public int id { get; set; }
        public string logo { get; set; }
        public string name { get; set; }
        public string originCountry { get; set; }
    }
}