using System;
using System.Collections.Generic;
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
        internal readonly DbSet<TEntity> DbSet;

        public ReadEntitySet(DbSet<TEntity> dbSet)
        {
            DbSet = dbSet;
        }

        private IQueryable<TEntity> ApplicableQueryable => DbSet;

        public IQueryable<TEntity> Many(
            Expression<Func<TEntity, bool>>? predicate, 
            params Expression<Func<TEntity, object>>[] includes)
        {
            var queryable = WithIncludes(includes);
            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            return queryable;
        }

        public Task<TEntity?> One(
            Expression<Func<TEntity, bool>> predicate, 
            params Expression<Func<TEntity, object>>[] includes)
        {
            return WithIncludes(includes).FirstOrDefaultAsync(predicate);
        }

        public async ValueTask<TEntity> OneById(params object[] keys)
        {
            return await DbSet.FindAsync(keys);
        }

        private IQueryable<TEntity> WithIncludes(IEnumerable<Expression<Func<TEntity, object>>> includes)
        {
            return includes.Aggregate(ApplicableQueryable, (current, expression) => current.Include(expression));
        }
        
        public Task<TProjection> ProjectOne<TProjection>(
            Expression<Func<TEntity, bool>> predicate, 
            Expression<Func<TEntity, TProjection>> projection,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return WithIncludes(includes).Where(predicate).Select(projection).FirstOrDefaultAsync();
        }
        
        public IQueryable<TProjection> ProjectMany<TProjection>(
            Expression<Func<TEntity, bool>>? predicate, 
            Expression<Func<TEntity, TProjection>> projection,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var queryable = WithIncludes(includes);
            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            return queryable.Select(projection).Distinct();
        }

    }
}