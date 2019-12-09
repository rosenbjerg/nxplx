using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;

namespace NxPlx.Abstractions.Database
{
    public interface IReadMediaContext : IReadContext<IMediaContext>
    {
        IReadEntitySet<FilmFile> FilmFiles { get; }
        IReadEntitySet<EpisodeFile> EpisodeFiles { get; }
        IReadEntitySet<Library> Libraries { get; }
    }
}