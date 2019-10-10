using System;
using System.Collections.Generic;
using System.Linq;

namespace NxPlx.Abstractions
{
    public abstract class MapperBase : IMapper
    {
        private readonly Dictionary<(Type, Type), object> _dictionary = new Dictionary<(Type, Type), object>();
        
        public void SetMapping<TFrom, TTo>(Func<TFrom, TTo> mapping)
            where TFrom : class
        {
            _dictionary[(typeof(TFrom), typeof(TTo))] = mapping;
        }
        public TTo? Map<TFrom, TTo>(TFrom instance)
            where TTo : class
            where TFrom : class
        {
            if (instance == default) return default;
            
            if (_dictionary.TryGetValue((typeof(TFrom), typeof(TTo)), out var mapperObject))
            {
                var mapper = (Func<TFrom, TTo>) mapperObject;
                return mapper(instance);
            }

            throw new ArgumentException($"No mapping from {typeof(TFrom).FullName} to {typeof(TTo).FullName}", nameof(instance));
        }
        public IEnumerable<TTo> MapMany<TFrom, TTo>(IEnumerable<TFrom> instances)
            where TFrom : class
        {
            if (instances == null) return Enumerable.Empty<TTo>();
            
            if (_dictionary.TryGetValue((typeof(TFrom), typeof(TTo)), out var mapperObject))
            {
                var mapper = (Func<TFrom, TTo>) mapperObject;
                return instances.Select(e => e == default ? default : mapper(e));
            }

            throw new ArgumentException($"No mapping from {typeof(TFrom).FullName} to {typeof(TTo).FullName}", nameof(instances));
        }
    }
}