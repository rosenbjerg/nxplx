using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Domain.Models;

namespace NxPlx.Infrastructure.Database
{
    public static class DbUtils
    {
        public static async Task AddOrUpdate<TEntity>(this DbContext context, IList<TEntity> entities)
            where TEntity : EntityBase
        {
            var set = context.Set<TEntity>();
            var ids = entities.Where(e => e.Id != default).Select(e => e.Id).ToList();
            var existing = await set.Where(e => ids.Contains(e.Id)).ToDictionaryAsync(e => e.Id);
            foreach (var entity in entities)
            {
                if (existing.TryGetValue(entity.Id, out var existingEntity))
                    context.Entry(existingEntity).CurrentValues.SetValues(entity);
                else
                    set.Add(entity);
            }
        }
    }
}