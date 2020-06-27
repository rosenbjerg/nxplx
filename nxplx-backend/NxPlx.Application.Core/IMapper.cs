using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NxPlx.Application.Core
{
    public interface IMapper
    {
        void SetMapping<TFrom, TTo>(Expression<Func<TFrom, TTo>> mapping)
            where TTo : class;

        Expression<Func<TFrom, TTo>>? GetProjectionExpression<TFrom, TTo>();
        
        TTo? Map<TFrom, TTo>(TFrom? instance)
            where TTo : class
            where TFrom : class;

        IEnumerable<TTo> Map<TFrom, TTo>(IEnumerable<TFrom> instances)
            where TTo : class;
    }
}