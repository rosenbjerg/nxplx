using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using Red;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class BroadcastRoutes
    {
        public static void Register(IRouter router)
        {
            router.WebSocket("/", Authenticated.User, Connect);
        }

        private static async Task<HandlerType> Connect(Request req, Response res, WebSocketDialog wsd)
        {
            var session = req.GetData<UserSession>();

            var broadcaster = ResolveContainer.Default().Resolve<IBroadcaster<WebSocketDialog>>();
            broadcaster.Subscribe(session.UserId, session.IsAdmin, wsd);

            return wsd.Continue();
        }
    }
}