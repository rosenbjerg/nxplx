using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.Details;
using NxPlx.Models.File;

namespace NxPlx.Abstractions.Database
{
    public interface INxplxContext : IContext
    {
        IEntitySet<FilmFile> FilmFiles { get; }
        IEntitySet<EpisodeFile> EpisodeFiles { get; }
        IEntitySet<Library> Libraries { get; }
        IEntitySet<SubtitlePreference> SubtitlePreferences { get; }
        IEntitySet<WatchingProgress> WatchingProgresses { get; }
        IEntitySet<User> Users { get; }
        IEntitySet<UserSession> UserSessions { get; }
        IEntitySet<Genre> Genres { get; }
    }
}