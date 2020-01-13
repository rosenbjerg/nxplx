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
            var id = keySelector(entity);
            if (!id.Equals(default))
            {
                var existingEntity = DbSet.FirstOrDefaultAsync(e => id.Equals(keySelector(e)));
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                DbSet.Add(entity);
            }
            
            var set = _context.Set<TEntity>();
            var existing = await set.FindAsync(keySelector(entity));
            
            if (existing == null) set.Add(entity);
            else set.Update(entity);
        }

        public async Task AddOrUpdate<TPrimaryKey>(IList<TEntity> entities, Func<TEntity, TPrimaryKey> keySelector)
        {
            var ids = entities.Select(keySelector).Where(id => !id.Equals(default)).ToList();
            var existing = await DbSet.Where(e => ids.Contains(keySelector(e))).ToDictionaryAsync(keySelector);
            foreach (var entity in entities)
            {
                if (existing.TryGetValue(keySelector(entity), out var existingEntity))
                    _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                else
                    DbSet.Add(entity);
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