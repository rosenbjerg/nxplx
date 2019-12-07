using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;

namespace NxPlx.Services.Database.Wrapper
{
    public class TransactionedUserContext : ReadUserContext, IUserContext
    {
        private readonly IDbContextTransaction _transaction;

        internal TransactionedUserContext(UserContext context) : base(context)
        {
            _subtitlePreferences = InitLazySet(context, context.SubtitlePreferences);
            _watchingProgresses = InitLazySet(context, context.WatchingProgresses);
            _users = InitLazySet(context, context.Users);
            _userSessions = InitLazySet(context, context.UserSessions);
            _transaction = context.Database.BeginTransaction();
        }
        
        private static Lazy<EntitySet<TEntity>> InitLazySet<TEntity>(DbContext context, DbSet<TEntity> set)
            where TEntity : class
        {
            return new Lazy<EntitySet<TEntity>>(() => new EntitySet<TEntity>(context, set));
        }
        
        private readonly Lazy<EntitySet<SubtitlePreference>> _subtitlePreferences;
        private readonly Lazy<EntitySet<WatchingProgress>> _watchingProgresses;
        private readonly Lazy<EntitySet<User>> _users;
        private readonly Lazy<EntitySet<UserSession>> _userSessions;

        public new IEntitySet<SubtitlePreference> SubtitlePreferences => _subtitlePreferences.Value;
        public new IEntitySet<WatchingProgress> WatchingProgresses => _watchingProgresses.Value;
        public new IEntitySet<User> Users => _users.Value;
        public new IEntitySet<UserSession> UserSessions => _userSessions.Value;

        public Task SaveChanges()
        {
            return Context.SaveChangesAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            await _transaction.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}