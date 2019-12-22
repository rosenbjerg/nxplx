using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.File;

namespace NxPlx.Services.Database.Wrapper
{
    public class ReadContext : IReadContext
    {
        protected readonly NxplxContext Context;

        public ReadContext() : this(new NxplxContext())
        {
            
        }

        private static Lazy<ReadEntitySet<TEntity>> InitLazyReadOnlySet<TEntity>(DbSet<TEntity> set)
            where TEntity : class
        {
            return new Lazy<ReadEntitySet<TEntity>>(() => new ReadEntitySet<TEntity>(set));
        }
        
        private readonly Lazy<ReadEntitySet<FilmFile>> _filmFiles;
        private readonly Lazy<ReadEntitySet<EpisodeFile>> _episodeFiles;
        private readonly Lazy<ReadEntitySet<Library>> _libraries;
        private readonly Lazy<ReadEntitySet<SubtitlePreference>> _subtitlePreferences;
        private readonly Lazy<ReadEntitySet<WatchingProgress>> _watchingProgresses;
        private readonly Lazy<ReadEntitySet<User>> _users;
        private readonly Lazy<ReadEntitySet<UserSession>> _userSessions;

        public IReadEntitySet<FilmFile> FilmFiles => _filmFiles.Value;
        public IReadEntitySet<EpisodeFile> EpisodeFiles => _episodeFiles.Value;
        public IReadEntitySet<Library> Libraries => _libraries.Value;
        public IReadEntitySet<SubtitlePreference> SubtitlePreferences => _subtitlePreferences.Value;
        public IReadEntitySet<WatchingProgress> WatchingProgresses => _watchingProgresses.Value;
        public IReadEntitySet<User> Users => _users.Value;
        public IReadEntitySet<UserSession> UserSessions => _userSessions.Value;

        public INxplxContext BeginTransactionedContext()
        {
            return new TransactionedContext(Context);
        }
        
        protected ReadContext(NxplxContext nxplxContext)
        {
            Context = nxplxContext;
            _filmFiles = InitLazyReadOnlySet(Context.FilmFiles);
            _episodeFiles = InitLazyReadOnlySet(Context.EpisodeFiles);
            _libraries = InitLazyReadOnlySet(Context.Libraries);
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