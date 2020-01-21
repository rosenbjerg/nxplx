using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.File;

namespace NxPlx.Services.Database.Wrapper
{
    public class TransactionedNxplxContext : ReadNxplxContext, INxplxContext
    {
        private readonly IDbContextTransaction _transaction;

        internal TransactionedNxplxContext(NxplxContext context) : base(context)
        {
            _transaction = context.Database.BeginTransaction();
            _filmFiles = InitLazySet(context, context.FilmFiles);
            _episodeFiles = InitLazySet(context, context.EpisodeFiles);
            _libraries = InitLazySet(context, context.Libraries);
            _subtitlePreferences = InitLazySet(context, context.SubtitlePreferences);
            _watchingProgresses = InitLazySet(context, context.WatchingProgresses);
            _users = InitLazySet(context, context.Users);
            _userSessions = InitLazySet(context, context.UserSessions);
            _genres = InitLazySet(context, context.Genre);
        }
        
        private static Lazy<EntitySet<TEntity>> InitLazySet<TEntity>(DbContext context, DbSet<TEntity> set)
            where TEntity : class
        {
            return new Lazy<EntitySet<TEntity>>(() => new EntitySet<TEntity>(context, set));
        }
        
        private readonly Lazy<EntitySet<FilmFile>> _filmFiles;
        private readonly Lazy<EntitySet<EpisodeFile>> _episodeFiles;
        private readonly Lazy<EntitySet<Library>> _libraries;
        private readonly Lazy<EntitySet<SubtitlePreference>> _subtitlePreferences;
        private readonly Lazy<EntitySet<WatchingProgress>> _watchingProgresses;
        private readonly Lazy<EntitySet<User>> _users;
        private readonly Lazy<EntitySet<UserSession>> _userSessions;
        private readonly Lazy<EntitySet<Genre>> _genres;

        public new IEntitySet<FilmFile> FilmFiles => _filmFiles.Value;
        public new IEntitySet<EpisodeFile> EpisodeFiles => _episodeFiles.Value;
        public new IEntitySet<Library> Libraries => _libraries.Value;
        public new IEntitySet<SubtitlePreference> SubtitlePreferences => _subtitlePreferences.Value;
        public new IEntitySet<WatchingProgress> WatchingProgresses => _watchingProgresses.Value;
        public new IEntitySet<User> Users => _users.Value;
        public new IEntitySet<UserSession> UserSessions => _userSessions.Value;
        public new IEntitySet<Genre> Genres => _genres.Value;

        public Task SaveChanges()
        {
            return Context.SaveChangesAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            try
            {
                await _transaction.CommitAsync();
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