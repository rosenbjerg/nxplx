using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        public async Task AddOrUpdate<TPrimaryKey>(TEntity entity, Func<TEntity, TPrimaryKey> keySelector)
        {
            var set = _context.Set<TEntity>();
            var existing = await set.FindAsync(keySelector(entity));
            
            if (existing == null) set.Add(entity);
            else set.Update(entity);
        }

        public async Task AddOrUpdate<TPrimaryKey>(IList<TEntity> entities, Expression<Func<TEntity, TPrimaryKey>> keySelector)
        {
            var set = _context.Set<TEntity>();
            var keySelectorFunc = keySelector.Compile();
            var ids = entities.Select(keySelectorFunc).ToList();
            var existingIds = await set.Where(e => ids.Contains(keySelectorFunc(e))).Select(keySelector).ToListAsync();

            var existingEntities = entities.Where(e => existingIds.Contains(keySelectorFunc(e))).ToList();
            set.AddRange(entities.Except(existingEntities));
            set.UpdateRange(existingEntities);
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