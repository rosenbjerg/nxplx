using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;

namespace NxPlx.Services.Database.Wrapper
{
    public class ReadUserContext : IReadUserContext
    {
        protected readonly UserContext Context;

        public ReadUserContext() : this(new UserContext())
        {
        }

        private static Lazy<ReadEntitySet<TEntity>> InitLazyReadOnlySet<TEntity>(DbSet<TEntity> set)
            where TEntity : class
        {
            return new Lazy<ReadEntitySet<TEntity>>(() => new ReadEntitySet<TEntity>(set));
        }
        
        private readonly Lazy<ReadEntitySet<SubtitlePreference>> _subtitlePreferences;
        private readonly Lazy<ReadEntitySet<WatchingProgress>> _watchingProgresses;
        private readonly Lazy<ReadEntitySet<User>> _users;
        private readonly Lazy<ReadEntitySet<UserSession>> _userSessions;

        public IReadEntitySet<SubtitlePreference> SubtitlePreferences => _subtitlePreferences.Value;
        public IReadEntitySet<WatchingProgress> WatchingProgresses => _watchingProgresses.Value;
        public IReadEntitySet<User> Users => _users.Value;
        public IReadEntitySet<UserSession> UserSessions => _userSessions.Value;

        public IUserContext BeginTransactionedContext()
        {
            return new TransactionedUserContext(Context);
        }
        
        protected ReadUserContext(UserContext userContext)
        {
            Context = userContext;
            _subtitlePreferences = InitLazyReadOnlySet(Context.SubtitlePreferences);
            _watchingProgresses = InitLazyReadOnlySet(Context.WatchingProgresses);
            _users = InitLazyReadOnlySet(Context.Users);
            _userSessions = InitLazyReadOnlySet(Context.UserSessions);
        }
        
        public virtual ValueTask DisposeAsync()
        {
            return Context.DisposeAsync();
        }
    }
}