using System.Threading.Tasks;

namespace NxPlx.Application.Models.Events
{
    public interface IEvent<TResult>
    {
    }

    public interface IQuery<TResult> : IEvent<TResult>
    {
    }
    
    public interface ICommand<TResult> : IEvent<TResult>
    {
    }    

    public interface ICommand : ICommand<Task>
    {
    }
}