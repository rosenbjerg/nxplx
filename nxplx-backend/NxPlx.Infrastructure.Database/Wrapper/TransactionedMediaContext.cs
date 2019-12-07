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

        internal TransactionedMediaContext(MediaContext context) : base(context)
        {
            _filmFiles = InitLazySet(context, context.FilmFiles);
            _episodeFiles = InitLazySet(context, context.EpisodeFiles);
            _libraries = InitLazySet(context, context.Libraries);
            _transaction = context.Database.BeginTransaction();
        }
        
        private static Lazy<EntitySet<TEntity>> InitLazySet<TEntity>(DbContext context, DbSet<TEntity> set)
            where TEntity : class
        {
            return new Lazy<EntitySet<TEntity>>(() => new EntitySet<TEntity>(context, set));
        }
        
        private readonly Lazy<EntitySet<FilmFile>> _filmFiles;
        private readonly Lazy<EntitySet<EpisodeFile>> _episodeFiles;
        private readonly Lazy<EntitySet<Library>> _libraries;

        public new IEntitySet<FilmFile> FilmFiles => _filmFiles.Value;
        public new IEntitySet<EpisodeFile> EpisodeFiles => _episodeFiles.Value;
        public new IEntitySet<Library> Libraries => _libraries.Value;

        public Task SaveChanges()
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