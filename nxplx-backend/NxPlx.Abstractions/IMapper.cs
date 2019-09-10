using System;
using System.Collections.Generic;

namespace NxPlx.Abstractions
{
    public interface IMapper
    {
        void SetMapping<TFrom, TTo>(Func<TFrom, TTo> mapping);

        TTo Map<TFrom, TTo>(TFrom instance)
            where TFrom : class;
        
        
        IEnumerable<TTo> MapMany<TFrom, TTo>(IEnumerable<TFrom> instances)
            where TFrom : class;
    }
}