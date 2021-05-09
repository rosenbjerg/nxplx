namespace NxPlx.Infrastructure.Events.Events
{
    public interface IDomainCommand<TResult> : IDomainEvent<TResult>
    {
    }
}