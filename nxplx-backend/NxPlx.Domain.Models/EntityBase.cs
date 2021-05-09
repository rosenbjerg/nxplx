using System;

namespace NxPlx.Domain.Models
{
    public abstract class EntityBase : ITrackedEntity
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid CreatedCorrelationId { get; set; }
        public Guid UpdatedCorrelationId { get; set; }
    }
}