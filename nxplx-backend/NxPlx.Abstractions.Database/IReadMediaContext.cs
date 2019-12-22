using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.File;

namespace NxPlx.Abstractions.Database
{
    public interface IReadContext : IReadContext<INxplxContext>
    {
        IReadEntitySet<FilmFile> FilmFiles { get; }
        IReadEntitySet<EpisodeFile> EpisodeFiles { get; }
        IReadEntitySet<Library> Libraries { get; }
        IReadEntitySet<SubtitlePreference> SubtitlePreferences { get; }
        IReadEntitySet<WatchingProgress> WatchingProgresses { get; }
        IReadEntitySet<User> Users { get; }
        IReadEntitySet<UserSession> UserSessions { get; }
    }
}