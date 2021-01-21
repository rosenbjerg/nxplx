using System.Threading.Tasks;

namespace NxPlx.Application.Models.Events
{
    public interface ICommand<TResult> : IEvent<TResult>
    {
    }    

    public interface ICommand : ICommand<Task>
    {
    }
}