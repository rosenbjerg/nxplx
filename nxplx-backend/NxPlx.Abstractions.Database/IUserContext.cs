using NxPlx.Infrastructure.Session;
using NxPlx.Models;

namespace NxPlx.Abstractions.Database
{
    public interface IUserContext : IContext
    {
        IEntitySet<SubtitlePreference> SubtitlePreferences { get; }
        IEntitySet<WatchingProgress> WatchingProgresses { get; }
        IEntitySet<User> Users { get; }
        IEntitySet<UserSession> UserSessions { get; }
    }
}