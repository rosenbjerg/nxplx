using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Dto.Models;
using NxPlx.Services.Database;
using Red;
using Red.Interfaces;

namespace NxPlx.WebApi.Routers
{
    public static class LibraryRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/libraries", Authenticated.User, ListLibraries);
            router.Post("/librarypermissions", Authenticated.Admin, SetUserLibraryPermissions);
        }
        
        private static async Task<HandlerType> ListLibraries(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var container = new ResolveContainer();
            await using var context = container.Resolve<MediaContext>();
            var libraries = await context.Libraries.ToListAsync();

            var mapper = container.Resolve<IDatabaseMapper>();
            
            if (session.IsAdmin)
                return await res.SendJson(mapper.MapMany<Library, AdminLibraryDto>(libraries));
            else
                return await res.SendJson(mapper.MapMany<Library, LibraryDto>(libraries));
        }
        private static async Task<HandlerType> SetUserLibraryPermissions(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();

            var userId = int.Parse(form["userId"]);
            var libraryIds = form["libraries"].Select(int.Parse).ToList();
            
            var container = new ResolveContainer();
            await using var context = container.Resolve<UserContext>();
            await using var mediaContext = container.Resolve<MediaContext>();
            
            var user = await context.Users.FindAsync(userId);

            if (user == default)
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }
            
            var libraries = await mediaContext.Libraries
                .Where(l => libraryIds.Contains(l.Id))
                .ToListAsync();
            
            if (libraryIds.Count != libraries.Count)
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            user.LibraryAccessIds.Clear();
            user.LibraryAccessIds.AddRange(libraryIds);
            await context.SaveChangesAsync();
            
            var mapper = container.Resolve<IDatabaseMapper>();
            return await res.SendJson(mapper.MapMany<Library, LibraryDto>(libraries));
        }
    }

}