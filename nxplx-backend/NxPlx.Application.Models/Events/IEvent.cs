namespace NxPlx.Application.Models.Events
{
    public interface IEvent
    {
    }
    public interface IEvent<TResult> : IEvent
    {
    }
}