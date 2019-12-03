using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions.Database;

namespace NxPlx.Services.Database.Wrapper
{
    public class EntitySet<TEntity> : ReadEntitySet<TEntity>, IEntitySet<TEntity>
        where TEntity : class
    {
        public EntitySet(ReadEntitySet<TEntity> readEntitySet) : base(readEntitySet.DbSet)
        {
            Tracked = true;
        }
        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }
        public void Add(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
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