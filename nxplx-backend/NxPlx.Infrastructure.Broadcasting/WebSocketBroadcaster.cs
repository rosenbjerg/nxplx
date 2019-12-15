using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NxPlx.Abstractions;
using Red;

namespace NxPlx.Infrastructure.Broadcasting
{
    public class WebSocketBroadcaster : IBroadcaster<WebSocketDialog>
    {
        private readonly List<WebsocketSession> _all = new List<WebsocketSession>();
        private readonly object _lock = new object();
        
        public Task BroadcastAdmin(object obj)
        {
            var message = JsonConvert.SerializeObject(obj);
            return Task.WhenAll(GetSessions(s => s.Admin).Select(dialog => dialog.SendText(message)));
        }

        public Task BroadcastAll(object obj)
        {
            var message = JsonConvert.SerializeObject(obj);
            return Task.WhenAll(GetSessions().Select(dialog => dialog.SendText(message)));
        }

        public Task BroadcastTo(int key, object obj)
        {
            var message = JsonConvert.SerializeObject(obj);
            return Task.WhenAll(GetSessions(s => s.UserId == key).Select(dialog => dialog.SendText(message)));
        }

        private IEnumerable<WebSocketDialog> GetSessions(Func<WebsocketSession, bool> predicate = null)
        {
            IList<WebsocketSession> receivers;
            lock (_lock)
            {
                receivers = predicate == null ? _all.ToList() : _all.Where(predicate).ToList();
            }
            return receivers.Select(r => r.WebSocketDialog);
        }

        public void Subscribe(int key, bool admin, WebSocketDialog channel)
        {
            var session = new WebsocketSession
            {
                UserId = key,
                WebSocketDialog = channel,
                Admin = admin
            };
            lock (_lock) _all.Add(session);
            channel.OnClosed += (sender, args) =>
            {
                lock (_lock) _all.Remove(session);
            };
        }
    }

    class WebsocketSession
    {
        public bool Admin;
        public int UserId;
        public WebSocketDialog WebSocketDialog;
    }
}