using System;
using System.Collections.Generic;
using System.Linq;

namespace NxPlx.Abstractions
{
    public abstract class MapperBase : IMapper
    {
        private readonly Dictionary<(Type, Type), object> _dictionary = new Dictionary<(Type, Type), object>();

        public void SetMapping<TFrom, TTo>(Func<TFrom, TTo> mapping)
            where TTo : class
        {
            _dictionary[(typeof(TFrom), typeof(TTo))] = mapping;
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

            if (_dictionary.TryGetValue((typeof(TFrom), typeof(TTo)), out var mapperObject))
            {
                var mapper = (Func<TFrom, TTo>) mapperObject;
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