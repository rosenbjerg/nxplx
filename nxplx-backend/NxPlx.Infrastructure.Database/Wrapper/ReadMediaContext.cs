using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions.Database;
using NxPlx.Models;
using NxPlx.Models.File;

namespace NxPlx.Services.Database.Wrapper
{
    public class ReadMediaContext : IReadMediaContext
    {
        internal readonly MediaContext Context;

        public ReadMediaContext() : this(new MediaContext())
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

        public IReadEntitySet<FilmFile> FilmFiles => _filmFiles.Value;
        public IReadEntitySet<EpisodeFile> EpisodeFiles => _episodeFiles.Value;
        public IReadEntitySet<Library> Libraries => _libraries.Value;
        
        public IMediaContext BeginTransactionedContext()
        {
            return new TransactionedMediaContext(this);
        }
        
        protected ReadMediaContext(MediaContext mediaContext)
        {
            Context = mediaContext;
            _filmFiles = InitLazyReadOnlySet(Context.FilmFiles);
            _episodeFiles = InitLazyReadOnlySet(Context.EpisodeFiles);
            _libraries = InitLazyReadOnlySet(Context.Libraries);
        }
        
        public virtual ValueTask DisposeAsync()
        {
            return Context.DisposeAsync();
        }
    }
}