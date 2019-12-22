using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class LibraryService
    {
        public static async Task<bool> SetUserLibraryPermissions(int userId, List<int> libraryIds)
        {
            var container = ResolveContainer.Default();
            await using var mediaContext = container.Resolve<IReadMediaContext>();
            await using var userContext = container.Resolve<IReadUserContext>();
            await using var transaction = userContext.BeginTransactionedContext();

            var user = await transaction.Users.OneById(userId);
            if (user == null) return false;

            var libraryCount = await mediaContext.Libraries.Count(l => libraryIds.Contains(l.Id));
            if (libraryIds.Count != libraryCount) return false;

            user.LibraryAccessIds = libraryIds;
            await transaction.SaveChanges();

            container.Resolve<ILoggingService>()
                .Info("Updated library permissions for user {Username}: {Permissions}", user.Username, string.Join(' ', user.LibraryAccessIds));
            return true;
        }


        public static IEnumerable<string>? GetDirectoryEntries(string cwd)
        {
            if (string.IsNullOrEmpty(cwd))
            {
                cwd = Path.GetPathRoot("/");
            }

            if (!Directory.Exists(cwd)) return null;

            return Directory.EnumerateDirectories(cwd, "*", new EnumerationOptions
            {
                AttributesToSkip = FileAttributes.Hidden | FileAttributes.Temporary | FileAttributes.System
            }).Select(Path.GetFileName);
        }
        public static async Task<bool> RemoveLibrary(int libraryId)
        {
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
                if (library == null) return false;

                if (library.Kind == LibraryKind.Film)
                {
                    var film = await mediaTransaction.FilmFiles.Many(f => f.PartOfLibraryId == libraryId);
                    var ids = film.Select(f => f.Id).ToList();
                    userTransaction.WatchingProgresses.Remove(
                        await userTransaction.WatchingProgresses.Many(wp => ids.Contains(wp.FileId)));
                    userTransaction.SubtitlePreferences.Remove(
                        await userTransaction.SubtitlePreferences.Many(wp => ids.Contains(wp.FileId)));
                    mediaTransaction.FilmFiles.Remove(film);
                }
                else
                {
                    var episodes = await mediaTransaction.EpisodeFiles.Many(f => f.PartOfLibraryId == libraryId);
                    var ids = episodes.Select(f => f.Id).ToList();
                    userTransaction.WatchingProgresses.Remove(
                        await userTransaction.WatchingProgresses.Many(wp => ids.Contains(wp.FileId)));
                    userTransaction.SubtitlePreferences.Remove(
                        await userTransaction.SubtitlePreferences.Many(wp => ids.Contains(wp.FileId)));
                    mediaTransaction.EpisodeFiles.Remove(episodes);
                }

                await userTransaction.SaveChanges();
                mediaTransaction.Libraries.Remove(library);
                await mediaTransaction.SaveChanges();
            }

            container.Resolve<ILoggingService>().Info("Deleted library {Username}", library.Name);
            return true;
        }
        public static async Task<User?> FindUser(int userId)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadUserContext>();

            return await context.Users.OneById(userId);
        }
        public static async Task<Library> CreateNewLibrary(string name, string path, string language, string kind)
        {
            var lib = new Library
            {
                Name = name,
                Path = path,
                Language = language,
                Kind = kind == "film" ? LibraryKind.Film : LibraryKind.Series
            };

            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadMediaContext>();
            await using var transaction = context.BeginTransactionedContext();

            transaction.Libraries.Add(lib);
            await transaction.SaveChanges();

            container.Resolve<ILoggingService>().Info("Created library {Name} with {Path}", lib.Name, lib.Path);
            return lib;
        }
    }
}