using System.Linq;

namespace NxPlx.Abstractions.Database
{
    public interface IEntitySet<TEntity> : IReadEntitySet<TEntity>, IWriteEntitySet<TEntity>
        where TEntity : class
    {
        
    }
}