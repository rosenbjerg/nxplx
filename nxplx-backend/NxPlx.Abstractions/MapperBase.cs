using System;
using System.Collections.Generic;

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
            if (_dictionary.TryGetValue((typeof(TFrom), typeof(TTo)), out var mapperObject))
            {
                var mapper = (Func<TFrom, TTo>) mapperObject;
                return mapper(instance);
            }

            return default;
        }
    }
}