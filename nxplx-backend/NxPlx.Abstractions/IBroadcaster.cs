using System.Threading.Tasks;

namespace NxPlx.Abstractions
{
    public interface IBroadcaster
    {
        Task BroadcastAdmin<T>(T obj);
        Task BroadcastAll<T>(T obj);
    }
    public interface IBroadcaster<TKey, TChannel> : IBroadcaster
    {
        Task BroadcastTo<T>(TKey key, T obj);

        void Unsubscribe(TKey key);
        void SubscribeAdmin(TKey key, TChannel channel);
        void SubscribeAll(TKey key, TChannel channel);
    }
}