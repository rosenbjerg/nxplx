namespace NxPlx.Infrastructure.Events.Events
{
    public interface IApplicationCommand<TResult> : IApplicationEvent<TResult>
    {
    }
}