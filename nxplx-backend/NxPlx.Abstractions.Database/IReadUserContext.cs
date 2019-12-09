using NxPlx.Infrastructure.Session;
using NxPlx.Models;

namespace NxPlx.Abstractions.Database
{
    public interface IReadUserContext : IReadContext<IUserContext>
    {
        IReadEntitySet<SubtitlePreference> SubtitlePreferences { get; }
        IReadEntitySet<WatchingProgress> WatchingProgresses { get; }
        IReadEntitySet<User> Users { get; }
        IReadEntitySet<UserSession> UserSessions { get; }
    }
}