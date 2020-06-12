using System.Collections.Generic;
using System.Threading.Tasks;

namespace NxPlx.Application.Core
{
    public interface IBroadcaster
    {
        Task BroadcastAdmin(object obj);
        Task BroadcastAll(object obj);
        Task BroadcastTo(int key, object obj);
        IReadOnlyList<int> UniqueIds();
    }
    public interface IBroadcaster<TChannel> : IBroadcaster
    {
        void Subscribe(int key, bool admin, TChannel channel);
    }
}