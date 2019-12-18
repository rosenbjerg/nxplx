using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
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
            if (string.IsNullOrEmpty(cwd))
            {
                cwd = Path.GetPathRoot("/");
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
            await using var context = container.Resolve<IReadMediaContext>();
            await using var transaction = context.BeginTransactionedContext();
            
            transaction.Libraries.Add(lib);
            await transaction.SaveChanges();

            container.Resolve<ILoggingService>()
                .Info("Created library {Name} with {Path}", lib.Name, lib.Path);

            return await res.SendMapped<Library, AdminLibraryDto>(container.Resolve<IDatabaseMapper>(), lib);
        }
        
        private static async Task<HandlerType> RemoveLibrary(Request req, Response res)
        {
            var libraryId = req.ParseBody<JsonValue<int>>().value;

            var container = ResolveContainer.Default();

            Library library;

            await using var readUserContext = container.Resolve<IReadUserContext>();
            await using var userTransaction = readUserContext.BeginTransactionedContext();
            foreach (var user in await userTransaction.Users.Many(u => u.LibraryAccessIds.Contains(libraryId)))
            {
                user.LibraryAccessIds.Remove(libraryId);
            }
            await userTransaction.SaveChanges();

            await using (var context = container.Resolve<IReadMediaContext>())
            await using (var mediaTransaction = context.BeginTransactionedContext())
            {
                library = await mediaTransaction.Libraries.OneById(libraryId);

                if (library.Kind == LibraryKind.Film)
                {
                    var film = await mediaTransaction.FilmFiles.Many(f => f.PartOfLibraryId == libraryId);
                    var ids = film.Select(f => f.Id).ToList();
                    userTransaction.WatchingProgresses.Remove(await userTransaction.WatchingProgresses.Many(wp => ids.Contains(wp.FileId)));
                    userTransaction.SubtitlePreferences.Remove(await userTransaction.SubtitlePreferences.Many(wp => ids.Contains(wp.FileId)));
                    mediaTransaction.FilmFiles.Remove(film);
                }
                else
                {
                    var episodes = await mediaTransaction.EpisodeFiles.Many(f => f.PartOfLibraryId == libraryId);
                    var ids = episodes.Select(f => f.Id).ToList();
                    userTransaction.WatchingProgresses.Remove(await userTransaction.WatchingProgresses.Many(wp => ids.Contains(wp.FileId)));
                    userTransaction.SubtitlePreferences.Remove(await userTransaction.SubtitlePreferences.Many(wp => ids.Contains(wp.FileId)));
                    mediaTransaction.EpisodeFiles.Remove(episodes);
                }
                
                await userTransaction.SaveChanges();
                mediaTransaction.Libraries.Remove(library);
                await mediaTransaction.SaveChanges();
            }
            
            container.Resolve<ILoggingService>()
                .Info("Deleted library {Username}", library.Name);

            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> ListLibraries(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var libraryAccess = session.User.LibraryAccessIds;
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadMediaContext>();

            var mapper = container.Resolve<IDatabaseMapper>();

            if (session.IsAdmin)
            {
                var libraries = await context.Libraries.Many();
                return await res.SendMapped<Library, AdminLibraryDto>(mapper, libraries);
            }
            else
            {
                var libraries = await context.Libraries.Many(l => libraryAccess.Contains(l.Id));
                return await res.SendMapped<Library, LibraryDto>(mapper, libraries);
            }
        }
        private static async Task<HandlerType> SetUserLibraryPermissions(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();

            var userId = int.Parse(form["userId"]);
            var libraryIds = form["libraries"].Select(int.Parse).ToList();
            
            var container = ResolveContainer.Default();
            await using var mediaContext = container.Resolve<IReadMediaContext>();
            await using var userContext = container.Resolve<IReadUserContext>();
            await using var transaction = userContext.BeginTransactionedContext();
            
            var user = await transaction.Users.OneById(userId);

            if (user == default)
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            var libraryCount = await mediaContext.Libraries
                .Count(l => libraryIds.Contains(l.Id));
            
            if (libraryIds.Count != libraryCount)
            {
                return await res.SendStatus(HttpStatusCode.BadRequest);
            }

            user.LibraryAccessIds = libraryIds;
            await transaction.SaveChanges();
            
            container.Resolve<ILoggingService>()
                .Info("Updated library permissions for user {Username}: {Permissions}", user.Username, string.Join(' ', user.LibraryAccessIds));

            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> GetUserLibraryPermissions(Request req, Response res)
        {
            var userId = int.Parse(req.Queries["userId"]);
            
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();
            
            var user = await context.Users.OneById(userId);

            if (user == default)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendJson(user.LibraryAccessIds);
        }
    }

}