using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Configuration;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using Red;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class BroadcastRoutes
    {
        public static void BindHandlers(IRouter router)
        {
            if (ConfigurationService.Current.Production) 
                router.WebSocket("/connect", Authenticated.User, Connect);
            else 
                router.WebSocket("/connect", Connect);
        }

        private static async Task<HandlerType> Connect(Request req, Response res, WebSocketDialog wsd)
        {
            var session = req.GetData<UserSession>();

            var broadcaster = ResolveContainer.Default.Resolve<IBroadcaster<WebSocketDialog>>();
            broadcaster.Subscribe(session.UserId, session.IsAdmin, wsd);

            return wsd.Final();
        }
    }
}