using NxPlx.Application.Models.Film;

namespace NxPlx.Application.Models.Events.Film
{
    public class CollectionDetailsLookupQuery : IQuery<MovieCollectionDto>
    {
        public CollectionDetailsLookupQuery(int collectionId)
        {
            CollectionId = collectionId;
        }

        public int CollectionId { get; }
    }
}