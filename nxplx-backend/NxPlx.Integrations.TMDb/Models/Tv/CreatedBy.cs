namespace NxPlx.Integrations.TMDb.Models.Tv
{
    public class CreatedBy
    {
        public int id { get; set; }
        public string credit_id { get; set; } = null!;
        public string name { get; set; } = null!;
        public int gender { get; set; }
        public string profile_path { get; set; } = null!;
    }
}