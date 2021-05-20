namespace NxPlx.Infrastructure.Events.Events
{
    public interface IDomainEvent<TResult> : IEvent<TResult>
    {
    }
}