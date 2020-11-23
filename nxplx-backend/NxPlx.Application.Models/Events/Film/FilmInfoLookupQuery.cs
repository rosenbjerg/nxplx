namespace NxPlx.Application.Models.Events.Film
{
    public class FilmInfoLookupQuery : IQuery<InfoDto>
    {
        public FilmInfoLookupQuery(int fileId)
        {
            FileId = fileId;
        }

        public int FileId { get; }
    }
}