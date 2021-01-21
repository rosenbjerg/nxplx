using NxPlx.Application.Models.Film;

namespace NxPlx.Application.Models.Events.Film
{
    public class FilmDetailsLookupQuery : IQuery<FilmDto?>
    {
        public FilmDetailsLookupQuery(int fileDetailsId)
        {
            FileDetailsId = fileDetailsId;
        }

        public int FileDetailsId { get; }
    }
}