using System.Threading.Tasks;
using NxPlx.Infrastructure.WebApi.Routes.Services;
using Red;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public class CommandRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/list", Authenticated.Admin, ListCommands);
            router.Post("/invoke", Authenticated.Admin, InvokeCommand);
        }

        private static Task<HandlerType> ListCommands(Request req, Response res)
        {
            var commands = AdminCommandService.AvailableCommands();
            return res.SendJson(commands);
        }
        private static Task<HandlerType> InvokeCommand(Request req, Response res)
        {
            string command = req.Queries["command"];
            string[] arguments = req.Queries["arguments"];

            var commands = AdminCommandService.InvokeCommand(command, arguments);
            
            return res.SendJson(commands);
        }
    }
}