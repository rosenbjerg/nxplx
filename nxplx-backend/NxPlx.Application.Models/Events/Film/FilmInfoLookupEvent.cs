using NxPlx.Models.Details.Film;

namespace NxPlx.Application.Models.Events
{
    public class FilmInfoLookupEvent : IEvent<InfoDto>
    {
        public FilmInfoLookupEvent(int fileId)
        {
            FileId = fileId;
        }

        public int FileId { get; }
    }
}