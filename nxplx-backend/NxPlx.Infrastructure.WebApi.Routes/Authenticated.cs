using System.Net;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Session;
using Red;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class Authenticated
    {
        public static async Task<HandlerType> User(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            if (session == default) 
                return await res.SendStatus(HttpStatusCode.Unauthorized);
            return HandlerType.Continue;
        }
        public static async Task<HandlerType> User(Request req, Response res, WebSocketDialog wsd)
        {
            var session = req.GetData<UserSession>();
            if (session == default) 
                return await res.SendStatus(HttpStatusCode.Unauthorized);
            return HandlerType.Continue;
        }
        
        public static async Task<HandlerType> Admin(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            if (session == default || !session.IsAdmin) 
                return await res.SendStatus(HttpStatusCode.Unauthorized);
            return HandlerType.Continue;
        }
    }
}