using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NxPlx.Abstractions.Database;
using NxPlx.Models;
using NxPlx.Models.File;

namespace NxPlx.Services.Database.Wrapper
{
    public class TransactionedMediaContext : ReadMediaContext, IMediaContext
    {
        private readonly IDbContextTransaction _transaction;

        internal TransactionedMediaContext(ReadMediaContext readMediaContext) : base(readMediaContext.Context)
        {
            _filmFiles = InitLazySet(readMediaContext.FilmFiles);
            _episodeFiles = InitLazySet(readMediaContext.EpisodeFiles);
            _libraries = InitLazySet(readMediaContext.Libraries);
            _transaction = readMediaContext.Context.Database.BeginTransaction();
        }
        
        private static Lazy<EntitySet<TEntity>> InitLazySet<TEntity>(IReadEntitySet<TEntity> set)
            where TEntity : class
        {
            return new Lazy<EntitySet<TEntity>>(() => new EntitySet<TEntity>(set as ReadEntitySet<TEntity>));
        }
        
        private readonly Lazy<EntitySet<FilmFile>> _filmFiles;
        private readonly Lazy<EntitySet<EpisodeFile>> _episodeFiles;
        private readonly Lazy<EntitySet<Library>> _libraries;

        public new IEntitySet<FilmFile> FilmFiles => _filmFiles.Value;
        public new IEntitySet<EpisodeFile> EpisodeFiles => _episodeFiles.Value;
        public new IEntitySet<Library> Libraries => _libraries.Value;

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