using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.File;

namespace NxPlx.Abstractions.Database
{
    public interface IReadNxplxContext : IReadContext<INxplxContext>
    {
        IReadEntitySet<FilmFile> FilmFiles { get; }
        IReadEntitySet<EpisodeFile> EpisodeFiles { get; }
        IReadEntitySet<Library> Libraries { get; }
        IReadEntitySet<SubtitlePreference> SubtitlePreferences { get; }
        IReadEntitySet<WatchingProgress> WatchingProgresses { get; }
        IReadEntitySet<User> Users { get; }
        IReadEntitySet<UserSession> UserSessions { get; }
        IReadEntitySet<Genre> Genres { get; }
        IReadEntitySet<MovieCollection> Collections { get; }
    }
}