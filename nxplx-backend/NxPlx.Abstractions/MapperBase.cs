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
        public void SetMergeMapping<TFrom1, TFrom2, TTo>(Func<TFrom1, TFrom2, TTo> mapping)
        {
            _dictionary[(typeof((TFrom1, TFrom2)), typeof(TTo))] = mapping;
        }

        public TTo Map<TFrom, TTo>(TFrom instance)
            where TFrom : class
        {
            if (instance == null) return default;
            
            if (_dictionary.TryGetValue((typeof(TFrom), typeof(TTo)), out var mapperObject))
            {
                var mapper = (Func<TFrom, TTo>) mapperObject;
                return mapper(instance);
            }

            throw new ArgumentException($"No mapping from {typeof(TFrom).FullName} to {typeof(TTo).FullName}", nameof(instance));
        }
        public TTo MergeMap<TFrom1, TFrom2, TTo>(TFrom1 instance1, TFrom2 instance2)
            where TFrom1 : class
            where TFrom2 : class
        {
            if (instance1 == null || instance2 == null) return default;
            
            if (_dictionary.TryGetValue((typeof((TFrom1, TFrom2)), typeof(TTo)), out var mapperObject))
            {
                var mapper = (Func<TFrom1, TFrom2, TTo>) mapperObject;
                return mapper(instance1, instance2);
            }

            throw new ArgumentException($"No merge-mapping from {typeof(TFrom1).FullName} and {typeof(TFrom2).FullName} to {typeof(TTo).FullName}", nameof(instance1));
        }
        
        public IEnumerable<TTo> MapMany<TFrom, TTo>(IEnumerable<TFrom> instances)
            where TFrom : class
        {
            if (instances == null) return new TTo[0];
            
            if (_dictionary.TryGetValue((typeof(TFrom), typeof(TTo)), out var mapperObject))
            {
                var mapper = (Func<TFrom, TTo>) mapperObject;
                return instances.Select(e => e == default ? default : mapper(e));
            }

            throw new ArgumentException($"No mapping from {typeof(TFrom).FullName} to {typeof(TTo).FullName}", nameof(instances));
        }
    }
}