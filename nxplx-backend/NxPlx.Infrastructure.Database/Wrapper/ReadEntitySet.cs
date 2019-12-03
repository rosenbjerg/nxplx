using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions.Database;

namespace NxPlx.Services.Database.Wrapper
{
    public class ReadEntitySet<TEntity> : IReadEntitySet<TEntity>
        where TEntity : class
    {
        protected bool Tracked = false;
        internal readonly DbSet<TEntity> DbSet;

        public ReadEntitySet(DbSet<TEntity> dbSet)
        {
            DbSet = dbSet;
        }

        private IQueryable<TEntity> ApplicableQueryable => Tracked ? DbSet : DbSet.AsNoTracking();
        
        public IQueryable<TEntity> Many(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return includes
                .Aggregate(ApplicableQueryable, (current, expression) => current.Include(expression))
                .Where(predicate);
        }
        public Task<TEntity?> One(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            return includes
                .Aggregate(ApplicableQueryable, (current, expression) => current.Include(expression))
                .FirstOrDefaultAsync(predicate);
        }

        public async ValueTask<TEntity> OneById(params object[] keys)
        {
            return await DbSet.FindAsync(keys);
        }
    }
}