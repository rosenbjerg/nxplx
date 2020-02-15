using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NxPlx.Abstractions.Database
{
    public interface IWriteEntitySet<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void Add(IEnumerable<TEntity> entities);

        Task AddOrUpdate<TPrimaryKey>(TEntity entity, Func<TEntity, TPrimaryKey> keySelector);
        Task AddOrUpdate<TPrimaryKey>(IList<TEntity> entities, Func<TEntity, TPrimaryKey> keySelector);
        
        void Remove(TEntity entity);
        void Remove(IEnumerable<TEntity> entities);
    }
}