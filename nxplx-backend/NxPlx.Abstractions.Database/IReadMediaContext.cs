using NxPlx.Models;
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