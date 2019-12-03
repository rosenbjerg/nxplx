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

        internal TransactionedUserContext(ReadUserContext readUserContext) : base(readUserContext.Context)
        {
            _subtitlePreferences = InitLazySet(readUserContext.SubtitlePreferences);
            _watchingProgresses = InitLazySet(readUserContext.WatchingProgresses);
            _users = InitLazySet(readUserContext.Users);
            _userSessions = InitLazySet(readUserContext.UserSessions);
            _transaction = readUserContext.Context.Database.BeginTransaction();
        }
        
        private static Lazy<EntitySet<TEntity>> InitLazySet<TEntity>(IReadEntitySet<TEntity> set)
            where TEntity : class
        {
            return new Lazy<EntitySet<TEntity>>(() => new EntitySet<TEntity>(set as ReadEntitySet<TEntity>));
        }
        
        private readonly Lazy<EntitySet<SubtitlePreference>> _subtitlePreferences;
        private readonly Lazy<EntitySet<WatchingProgress>> _watchingProgresses;
        private readonly Lazy<EntitySet<User>> _users;
        private readonly Lazy<EntitySet<UserSession>> _userSessions;

        public new IEntitySet<SubtitlePreference> SubtitlePreferences => _subtitlePreferences.Value;
        public new IEntitySet<WatchingProgress> WatchingProgresses => _watchingProgresses.Value;
        public new IEntitySet<User> Users => _users.Value;
        public new IEntitySet<UserSession> UserSessions => _userSessions.Value;

        public Task Commit()
        {
            return _transaction.CommitAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                await _transaction.RollbackAsync();
            }

            await _transaction.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}