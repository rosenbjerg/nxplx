namespace NxPlx.Integrations.TMDb.Models
{
    public class ProductionCompany
    {
        public int id { get; set; }
        public string logo_path { get; set; } = null!;
        public string name { get; set; } = null!;
        public string origin_country { get; set; } = null!;
    }
}