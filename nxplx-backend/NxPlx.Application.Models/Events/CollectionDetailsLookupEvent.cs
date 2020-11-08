using NxPlx.Application.Models.Film;

namespace NxPlx.Application.Models.Events
{
    public class CollectionDetailsLookupEvent : IEvent<MovieCollectionDto>
    {
        public CollectionDetailsLookupEvent(int collectionId)
        {
            CollectionId = collectionId;
        }

        public int CollectionId { get; set; }
    }
}