using System.Linq;

namespace NxPlx.Application.Core
{
    public static class MappingExtensions
    {
        public static IQueryable<TTo> Project<TFrom, TTo>(this IQueryable<TFrom> queryable, IMapper mapper)
        {
            var projectionExpression = mapper.GetProjectionExpression<TFrom, TTo>();
            return queryable.Select(projectionExpression);
        }
    }
}