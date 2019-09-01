using NxPlx.Models.Details;

namespace NxPlx.Models.Database
{
    public class InGenre
    {
        
        public int DetailsEntityId { get; set; }
        public DetailsEntityBase DetailsEntity { get; set; }
        
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}