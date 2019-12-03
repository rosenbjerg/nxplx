using System.Collections.Generic;
using System.Threading.Tasks;

namespace NxPlx.Abstractions.Database
{
    public interface IWriteEntitySet<in TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void Add(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void Remove(IEnumerable<TEntity> entities);
    }
}