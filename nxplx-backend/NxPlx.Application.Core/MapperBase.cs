using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NxPlx.Application.Core
{
    public static class MappingExtensions
    {
        public static IQueryable<TTo> Project<TFrom, TTo>(this IQueryable<TFrom> queryable, IMapper mapper)
        {
            var projectionExpression = mapper.GetProjectionExpression<TFrom, TTo>();
            return queryable.Select(projectionExpression);
        }
    }
    public abstract class MapperBase : IMapper
    {
        private readonly Dictionary<(Type, Type), object> _expressions = new();
        private readonly Dictionary<(Type, Type), object> _compiledCache = new();

        public void SetMapping<TFrom, TTo>(Expression<Func<TFrom, TTo>> mapping)
            where TTo : class
        {
            _expressions[(typeof(TFrom), typeof(TTo))] = mapping;
        }
        
        public Expression<Func<TFrom, TTo>> GetProjectionExpression<TFrom, TTo>()
        {
            return (Expression<Func<TFrom, TTo>>)_expressions[(typeof(TFrom), typeof(TTo))];
        }
        
        public TTo? Map<TFrom, TTo>(TFrom? instance)
            where TTo : class
            where TFrom : class
        {
            if (instance == null) return null;

            return Map<TFrom, TTo>(new[] {instance}).First();
        }

        public IEnumerable<TTo> Map<TFrom, TTo>(IEnumerable<TFrom> instances)
            where TTo : class
        {
            if (instances == null) return Enumerable.Empty<TTo>();
            var key = (typeof(TFrom), typeof(TTo));

            if (_expressions.TryGetValue(key, out var mapperObject))
            {
                if (!_compiledCache.TryGetValue(key, out var cached))
                    _compiledCache[key] = cached = ((Expression<Func<TFrom, TTo>>)mapperObject).Compile();

                var mapper = (Func<TFrom, TTo>)cached;
                return instances.Aggregate(new List<TTo>(), (acc, cur) =>
                {
                    if (cur != null) acc.Add(mapper(cur));
                    return acc;
                });
            }

            throw new ArgumentException($"No mapping from {typeof(TFrom).FullName} to {typeof(TTo).FullName}",
                nameof(instances));
        }
    }
}