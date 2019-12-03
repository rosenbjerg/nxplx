using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NxPlx.Abstractions.Database
{
    public interface IReadEntitySet<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Many(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        
        Task<TEntity?> One(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        ValueTask<TEntity> OneById(params object[] keys);
    }
}