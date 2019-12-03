using NxPlx.Models;
using NxPlx.Models.File;

namespace NxPlx.Abstractions.Database
{
    public interface IMediaContext : IContext
    {
        IEntitySet<FilmFile> FilmFiles { get; }
        IEntitySet<EpisodeFile> EpisodeFiles { get; }
        IEntitySet<Library> Libraries { get; }
    }
}