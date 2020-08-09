using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NxPlx.Infrastructure.Broadcasting
{
    public abstract class ConnectionAccepter
    {
        private readonly ConnectionHub _connectionHub;

        protected ConnectionAccepter(ConnectionHub connectionHub)
        {
            _connectionHub = connectionHub;
        }

        protected void Connect(Connection connection)
        {
            _connectionHub.Add(connection);
        }

        public abstract Task Accept(HttpContext httpContext);
    }
}