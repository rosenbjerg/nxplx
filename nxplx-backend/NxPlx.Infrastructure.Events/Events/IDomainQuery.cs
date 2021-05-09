namespace NxPlx.Infrastructure.Events.Events
{
    public interface IDomainQuery<TResult> : IDomainEvent<TResult>
    {
    }
}