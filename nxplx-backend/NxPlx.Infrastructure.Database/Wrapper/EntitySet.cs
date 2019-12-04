using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions.Database;

namespace NxPlx.Services.Database.Wrapper
{
    public class EntitySet<TEntity> : ReadEntitySet<TEntity>, IEntitySet<TEntity>
        where TEntity : class
    {
        private readonly DbContext _context;

        public EntitySet(DbContext context, DbSet<TEntity> dbSet) : base(dbSet)
        {
            _context = context;
        }
        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }
        public void Add(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public void AddOrUpdate<TPrimaryKey>(TEntity entity, Func<TEntity, TPrimaryKey> keySelector)
        {
            _context.Entry(entity).State = keySelector(entity).Equals(default) ?
                EntityState.Added :
                EntityState.Modified;
        }

        public void AddOrUpdate<TPrimaryKey>(IEnumerable<TEntity> entities, Func<TEntity, TPrimaryKey> keySelector)
        {
            foreach (var entity in entities)
            {
                AddOrUpdate(entity, keySelector);
            }
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }
        public void Remove(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}