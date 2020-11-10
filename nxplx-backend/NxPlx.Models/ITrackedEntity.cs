using System;

namespace NxPlx.Models
{
    public interface ITrackedEntity
    {
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
        Guid CreatedCorrelationId { get; set; }
        Guid UpdatedCorrelationId { get; set; }
    }
}