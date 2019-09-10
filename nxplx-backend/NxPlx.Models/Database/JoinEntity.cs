using NxPlx.Models;

namespace NxPlx.Services.Database.Models
{
    public class JoinEntity<TEntity1, TEntity2>
        where TEntity1 : EntityBase
        where TEntity2 : EntityBase
    {
        public virtual TEntity1 Entity1 { get; set; }
        public int Entity1Id { get; set; }
        
        public virtual TEntity2 Entity2 { get; set; }
        public int Entity2Id { get; set; }
    }
    
    public class JoinEntity<TEntity1, TEntity2, TEntity2Key>
        where TEntity1 : EntityBase
        where TEntity2 : class
    {
        public virtual TEntity1 Entity1 { get; set; }
        public int Entity1Id { get; set; }
        
        public virtual TEntity2 Entity2 { get; set; }
        public TEntity2Key Entity2Id { get; set; }
    }
}