using System;
using System.Collections.Generic;

namespace NxPlx.Abstractions
{
    public interface IMapper
    {
        void SetMapping<TFrom, TTo>(Func<TFrom, TTo> mapping)
            where TFrom : class;

        TTo? Map<TFrom, TTo>(TFrom instance)
            where TTo : class
            where TFrom : class;
        
        IEnumerable<TTo> Map<TFrom, TTo>(IEnumerable<TFrom> instances)
            where TFrom : class;
    }
}