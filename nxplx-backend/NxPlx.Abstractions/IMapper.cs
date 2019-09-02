using System;

namespace NxPlx.Abstractions
{
    public interface IMapper
    {
        void SetMapping<TFrom, TTo>(Func<TFrom, TTo> mapping);

        TTo Map<TFrom, TTo>(TFrom instance)
            where TFrom : class;
    }
}