using NxPlx.Application.Models.Film;

namespace NxPlx.Application.Models.Events
{
    public class FilmDetailsLookupEvent : IEvent<FilmDto?>
    {
        public FilmDetailsLookupEvent(int fileDetailsId)
        {
            FileDetailsId = fileDetailsId;
        }

        public int FileDetailsId { get; }
    }
}