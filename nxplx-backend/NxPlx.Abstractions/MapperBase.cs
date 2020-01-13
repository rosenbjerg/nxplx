using System;
using System.Collections.Generic;
using System.Linq;

namespace NxPlx.Abstractions
{
    public abstract class MapperBase : IMapper
    {
        private readonly Dictionary<(Type, Type), object> _dictionary = new Dictionary<(Type, Type), object>();
        
        public void SetMapping<TFrom, TTo>(Func<TFrom, TTo> mapping)
        {
            _dictionary[(typeof(TFrom), typeof(TTo))] = mapping;
        }
        public TTo Map<TFrom, TTo>(TFrom instance)
        {
            if (instance.Equals(default)) return default;

            return Map<TFrom, TTo>(new[] { instance }).First();
        }
        public IEnumerable<TTo> Map<TFrom, TTo>(IEnumerable<TFrom> instances)
        {
            if (instances == null || !instances.Any()) return Enumerable.Empty<TTo>();
            
            if (_dictionary.TryGetValue((typeof(TFrom), typeof(TTo)), out var mapperObject))
            {
                var mapper = (Func<TFrom, TTo>) mapperObject;
                return instances.Select(e => e.Equals(default) ? default : mapper(e));
            }

            throw new ArgumentException($"No mapping from {typeof(TFrom).FullName} to {typeof(TTo).FullName}", nameof(instances));
        }
    }
}