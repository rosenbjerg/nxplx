using System;
using System.Collections.Generic;

namespace NxPlx.Abstractions.Database
{
    public interface IWriteEntitySet<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void Add(IEnumerable<TEntity> entities);

        void AddOrUpdate<TPrimaryKey>(TEntity entity, Func<TEntity, TPrimaryKey> keySelector);
        void AddOrUpdate<TPrimaryKey>(IEnumerable<TEntity> entities, Func<TEntity, TPrimaryKey> keySelector);
        
        void Remove(TEntity entity);
        void Remove(IEnumerable<TEntity> entities);
    }
}