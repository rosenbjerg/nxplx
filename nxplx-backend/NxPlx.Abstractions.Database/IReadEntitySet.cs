using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NxPlx.Abstractions.Database
{
    public interface IReadEntitySet<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> Many(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includes);
        
        Task<TEntity?> One(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        ValueTask<TEntity?> OneById(params object[] keys);
        Task<TProjection> ProjectOne<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> projection, params Expression<Func<TEntity, object>>[] includes);
        IQueryable<TProjection> ProjectMany<TProjection>(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, TProjection>> projection, params Expression<Func<TEntity, object>>[] includes);
    }
}