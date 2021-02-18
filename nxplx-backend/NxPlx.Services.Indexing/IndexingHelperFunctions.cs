using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Services.Index
{
    internal static class IndexingHelperFunctions
    {
        internal static Task<List<T>> GetUniqueNew<T>(this IEnumerable<T> entities, DatabaseContext databaseContext)
            where T : EntityBase
            => GetUniqueNew(entities, e => e.Id, databaseContext);

        internal static async Task<List<T>> GetUniqueNew<T, TKey>(this IEnumerable<T> entities, Expression<Func<T, TKey>> keySelector, DatabaseContext databaseContext)
            where T : class
            where TKey : notnull
        {
            var keySelectorFunc = keySelector.Compile();
            var unique = GetUnique(entities, keySelectorFunc);
            return await GetNew(unique, keySelector, databaseContext);
        }
        
        private static async Task<List<T>> GetNew<T, TKey>(this IEnumerable<T> entities, Expression<Func<T, TKey>> keySelector, DatabaseContext databaseContext)
            where T : class
        {
            List<TKey> unique;
            unique = await databaseContext.Set<T>().Select(keySelector).ToListAsync();

            var uniqueHashset = unique.ToHashSet();

            var keySelectorFunc = keySelector.Compile();
            return entities.Where(e => !uniqueHashset.Contains(keySelectorFunc(e))).ToList();
        }

        private static List<T> GetUnique<T, TKey>(this IEnumerable<T> entities, Func<T, TKey> keySelector)
            where T : notnull
            where TKey : notnull
        {
            var unique = new Dictionary<TKey, T>();
            foreach (var entity in entities)
            {
                if (entity == null) 
                    continue;
                var key = keySelector(entity);
                unique[key] = entity;
            }

            return unique.Values.ToList();
        }
    }
}