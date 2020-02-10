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
        public static void Register(IRouter router)
        {
            if (ConfigurationService.Current.Production) 
                router.WebSocket("/connect", Authenticated.User, Connect);
            else 
                router.WebSocket("/connect", Connect);
        }

        public static async Task<HandlerType> Echo(Request req, Response res, WebSocketDialog wsd)
        {
            await wsd.SendText("Welcome to the echo test server");
            wsd.OnTextReceived += (sender, eventArgs) => { wsd.SendText("you sent: " + eventArgs.Text); };
            return HandlerType.Final;
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