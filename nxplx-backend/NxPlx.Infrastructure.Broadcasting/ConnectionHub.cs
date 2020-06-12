using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Application.Core;
using NxPlx.Models;

namespace NxPlx.Infrastructure.Broadcasting
{
    public class ConnectionHub
    {
        private readonly ConcurrentDictionary<int, UserConnections> _connectionTable = new ConcurrentDictionary<int, UserConnections>();
        private readonly object _tableLock = new object();
        
        public void Add(Connection connection)
        {
            var userConnections = _connectionTable.GetOrAdd(connection.OperationContext.User.Id, _ => new UserConnections(connection.OperationContext.User));
            lock (userConnections.Sync) userConnections.Connections.Add(connection);
            connection.Disconnected += OnDisconnected;
            connection.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, Message msg)
        {
            var connection = (Connection)sender;
            
        }

        public int[] ConnectedIds() => _connectionTable.Keys.ToArray();

        public Task SendMessage(int userId, Message message)
        {
            var userConnections = _connectionTable[userId];
            List<Connection> copy;
            lock (_tableLock) lock (userConnections.Sync) copy = userConnections.Connections.ToList();
            return Task.WhenAll(copy.Select(connection => connection.SendMessage(message)));
        }
        public Task Broadcast(Message message)
        {
            List<Connection> copy;
            lock (_tableLock) copy = _connectionTable.Values.SelectMany(pair => pair.Connections).ToList();
            return Task.WhenAll(copy.Select(connection => connection.SendMessage(message)));
        }
        public Task BroadcastAdmins(Message message)
        {
            List<Connection> copy;
            lock (_tableLock)
                copy = _connectionTable.Values
                    .Where(userConnections => userConnections.Admin)
                    .SelectMany(userConnections =>
                    {
                        lock (userConnections.Sync) return userConnections.Connections.ToList();
                    }).ToList();
            return Task.WhenAll(copy.Select(connection => connection.SendMessage(message)));
        }
        
        private void OnDisconnected(object sender, EventArgs e)
        {
            var connection = (Connection)sender;
            connection.Disconnected -= OnDisconnected;
            connection.MessageReceived -= OnMessageReceived;
            var userConnections = _connectionTable[connection.OperationContext.User.Id];
            lock (userConnections.Sync) userConnections.Connections.Remove(connection);
        }

        class UserConnections
        {
            public UserConnections(User user)
            {
                Admin = user.Admin;
            }
            internal readonly object Sync = new object();
            internal readonly List<Connection> Connections = new List<Connection>();
            internal bool Admin;
        }
    }
}