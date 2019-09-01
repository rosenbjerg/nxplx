using System;
using System.Collections.Generic;

namespace NxPlx.Abstractions
{
    public interface IDetailsMapper : IMapper
    {
        List<TPropertyEntity> UniqueProperties<TRootEntity, TPropertyEntity, TPropertyKey>(
            IEnumerable<TRootEntity> entities,
            Func<TRootEntity, IEnumerable<TPropertyEntity>> selector,
            Func<TPropertyEntity, TPropertyKey> keySelector)
            where TRootEntity : class
            where TPropertyEntity : class;

        List<TPropertyEntity> UniqueProperties<TRootEntity, TPropertyEntity, TPropertyKey>(
            IEnumerable<TRootEntity> entities,
            Func<TRootEntity, TPropertyEntity> selector,
            Func<TPropertyEntity, TPropertyKey> keySelector)
            where TRootEntity : class
            where TPropertyEntity : class;
    }
}