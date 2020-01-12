using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Infrastructure.WebApi.Routes.Services;
using NxPlx.Models;
using NxPlx.Models.Dto.Models;
using Red;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class LibraryRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/list", Authenticated.User, ListLibraries);
            router.Post("", Authenticated.Admin, CreateLibrary);
            router.Delete("", Authenticated.Admin, RemoveLibrary);
            router.Get("/permissions", Validated.RequireUserIdQuery, Authenticated.Admin, GetUserLibraryPermissions);
            router.Put("/permissions", Validated.SetUserPermissionsForm, Authenticated.Admin, SetUserLibraryPermissions);
            router.Get("/browse", Authenticated.Admin, BrowseForDirectory);
        }
        
        
        private static Task<HandlerType> BrowseForDirectory(Request req, Response res)
        { 
            string cwd = req.Queries["cwd"];
            
            var entries = LibraryService.GetDirectoryEntries(cwd);
            
            if (entries == null) return res.SendStatus(HttpStatusCode.BadRequest);
            return res.SendJson(entries);
        }


        private static async Task<HandlerType> CreateLibrary(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            
            var lib = await LibraryService.CreateNewLibrary(form["name"], form["path"], form["language"], form["kind"]);

            return await res.SendJson(lib);
        }


        private static async Task<HandlerType> RemoveLibrary(Request req, Response res)
        {
            var libraryId = req.ParseBody<JsonValue<int>>().value;

            var ok = await LibraryService.RemoveLibrary(libraryId);

            if (!ok) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendStatus(HttpStatusCode.OK);
        }


        private static async Task<HandlerType> ListLibraries(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var libraries = await ListLibraries(session.IsAdmin, session.User.LibraryAccessIds);
            return await res.SendJson(libraries);
        }

        private static async Task<IEnumerable<LibraryDto>> ListLibraries(bool isAdmin, IEnumerable<int> libraryAccess)
        {
            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>();

            if (isAdmin)
            {
                var libraries = await context.Libraries.Many().ToListAsync();
                return container.Resolve<IDtoMapper>().Map<Library, AdminLibraryDto>(libraries);
            }
            else
            {
                var libraries = await context.Libraries.Many(l => libraryAccess.Contains(l.Id)).ToListAsync();
                return container.Resolve<IDtoMapper>().Map<Library, LibraryDto>(libraries);
            }
        } 
        private static async Task<HandlerType> SetUserLibraryPermissions(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            var userId = int.Parse(form["userId"]);
            var libraryIds = form["libraries"].Select(int.Parse).ToList();
            
            var ok = await LibraryService.SetUserLibraryPermissions(userId, libraryIds);

            if (!ok) return await res.SendStatus(HttpStatusCode.BadRequest);
            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> GetUserLibraryPermissions(Request req, Response res)
        {
            var userId = int.Parse(req.Queries["userId"]);
            var libraryAccess = await LibraryService.FindLibraryAccess(userId);
            if (libraryAccess == default) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendJson(libraryAccess);
        }


    }
}