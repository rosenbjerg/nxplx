using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.Dto.Models;
using NxPlx.Services.Database;
using Red;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.WebApi.Routes
{
    public static class LibraryRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/list", Authenticated.User, ListLibraries);
            router.Post("", Authenticated.User, CreateLibrary);
            router.Delete("", Authenticated.User, RemoveLibrary);
            router.Get("/permissions", Validated.GetUserPermissionsForm, Authenticated.Admin, GetUserLibraryPermissions);
            router.Post("/permissions", Validated.SetUserPermissionsForm, Authenticated.Admin, SetUserLibraryPermissions);
            router.Get("/browse", Authenticated.Admin, BrowseForDirectory);
        }
        
        
        private static Task<HandlerType> BrowseForDirectory(Request req, Response res)
        { 
            string cwd = req.Queries["cwd"];
            if (string.IsNullOrEmpty(cwd))
            {
                cwd = "/";
            }

            if (!Directory.Exists(cwd))
            {
                return res.SendStatus(HttpStatusCode.BadRequest);
            }

            return res.SendJson(Directory.EnumerateDirectories(cwd, "*", new EnumerationOptions
            {
                AttributesToSkip = FileAttributes.Hidden | FileAttributes.Temporary | FileAttributes.System
            }).Select(Path.GetFileName));
        } 
        
        private static async Task<HandlerType> CreateLibrary(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            
            var lib = new Library
            {
                Name = form["name"],
                Path = form["path"],
                Language = form["language"],
                Kind = form["kind"] == "film" ? LibraryKind.Film : LibraryKind.Series
            };

            var container = ResolveContainer.Default();
            await using var context = container.Resolve<MediaContext>();
            await context.AddAsync(lib);
            await context.SaveChangesAsync();

            container.Resolve<ILoggingService>()
                .Info("Created library {Name} with {Path}", lib.Name, lib.Path);

            return await res.SendMapped<Library, AdminLibraryDto>(container.Resolve<IDatabaseMapper>(), lib);
        }
        
        private static async Task<HandlerType> RemoveLibrary(Request req, Response res)
        {
            var libraryId = req.ParseBody<JsonValue<int>>().value;

            var container = ResolveContainer.Default();

            Library library;
            
            await using (var context = container.Resolve<UserContext>())
            {
                var users = await context.Users.ToListAsync();
                foreach (var user in users)
                {
                    user.LibraryAccessIds.Remove(libraryId);
                }
                await context.SaveChangesAsync();
            }
            
            await using (var context = container.Resolve<MediaContext>())
            {
                library = await context.Libraries.FindAsync(libraryId);

                if (library.Kind == LibraryKind.Film)
                {
                    var film = await context.FilmFiles.Where(f => f.PartOfLibraryId == libraryId).ToListAsync();
                    context.RemoveRange(film);
                }
                else
                {
                    var episodes = await context.EpisodeFiles.Where(f => f.PartOfLibraryId == libraryId).ToListAsync();
                    context.RemoveRange(episodes);
                }
                
                context.Remove(library);
                await context.SaveChangesAsync();
            }
            
            container.Resolve<ILoggingService>()
                .Info("Deleted library {Username}", library.Name);

            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> ListLibraries(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<MediaContext>();

            var mapper = container.Resolve<IDatabaseMapper>();

            if (session.IsAdmin)
            {
                var libraries = await context.Libraries.ToListAsync();
                return await res.SendMapped<Library, AdminLibraryDto>(mapper, libraries);
            }
            else
            {
                var libraries = await context.Libraries
                    .Where(l => session.LibraryAccess.Contains(l.Id))
                    .ToListAsync();
                return await res.SendMapped<Library, LibraryDto>(mapper, libraries);
            }
        }
        private static async Task<HandlerType> SetUserLibraryPermissions(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();

            var userId = int.Parse(form["userId"]);
            var libraryIds = form["libraries"].Select(int.Parse).ToList();
            
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();
            await using var mediaContext = container.Resolve<MediaContext>();
            
            var user = await context.Users.FindAsync(userId);

            if (user == default)
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            var libraryCount = await mediaContext.Libraries
                .Where(l => libraryIds.Contains(l.Id))
                .CountAsync();
            
            if (libraryIds.Count != libraryCount)
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            user.LibraryAccessIds.Clear();
            user.LibraryAccessIds.AddRange(libraryIds);
            await context.SaveChangesAsync();
            
            container.Resolve<ILoggingService>()
                .Info("Updated library permissions for user {Username}: ", user.Username, string.Join(' ', user.LibraryAccessIds));

            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> GetUserLibraryPermissions(Request req, Response res)
        {
            string userId = req.Queries["userId"];
            
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();
            
            var user = await context.Users.FindAsync(userId);

            if (user == default)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendJson(user.LibraryAccessIds);
        }
    }

}