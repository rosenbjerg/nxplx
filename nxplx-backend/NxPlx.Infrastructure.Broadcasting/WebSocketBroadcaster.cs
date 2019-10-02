using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NxPlx.Abstractions;
using Red;

namespace NxPlx.Infrastructure.Broadcasting
{
    public class WebSocketBroadcaster : IBroadcaster<string, WebSocketDialog>, IBroadcaster
    {
        private ConcurrentDictionary<string, WebSocketDialog> _all = new ConcurrentDictionary<string, WebSocketDialog>();
        private ConcurrentDictionary<string, WebSocketDialog> _admin = new ConcurrentDictionary<string, WebSocketDialog>();
        
        public Task BroadcastAdmin<T>(T obj)
        {
            var message = JsonConvert.SerializeObject(obj);
            return Task.WhenAll(_admin.Values.Select(channel => channel.SendText(message)));
        }

        public Task BroadcastAll<T>(T obj)
        {
            var message = JsonConvert.SerializeObject(obj);
            return Task.WhenAll(_all.Values.Select(channel => channel.SendText(message)));
        }

        public Task BroadcastTo<T>(string key, T obj)
        {
            var message = JsonConvert.SerializeObject(obj);
            return _all[key].SendText(message);
        }

        public void Unsubscribe(string key)
        {
            _all.TryRemove(key, out _);
            _admin.TryRemove(key, out _);
        }

        public void SubscribeAdmin(string key, WebSocketDialog channel)
        {
            _admin[key] = channel;
            _all[key] = channel;
            channel.OnClosed += (sender, args) => Unsubscribe(key);
        }

        public void SubscribeAll(string key, WebSocketDialog channel)
        {
            _all[key] = channel;
        }
    }
}