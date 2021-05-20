namespace NxPlx.Integrations.TMDb.Models.Movie
{
    public class MovieCollection
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string poster_path { get; set; } = null!;
        public string backdrop_path { get; set; } = null!;
    }
}