using System.Threading.Tasks;

namespace NxPlx.Abstractions
{
    public interface IBroadcaster
    {
        Task BroadcastAdmin(object obj);
        Task BroadcastAll(object obj);
        Task BroadcastTo(int key, object obj);
    }
    public interface IBroadcaster<TChannel> : IBroadcaster
    {
        void Subscribe(int key, bool admin, TChannel channel);
    }
}