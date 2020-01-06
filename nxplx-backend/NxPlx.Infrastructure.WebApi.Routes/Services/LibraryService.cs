using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using NxPlx.Models.Dto.Models;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class LibraryService
    {
        public static async Task<bool> SetUserLibraryPermissions(int userId, List<int> libraryIds)
        {
            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>();
            await using var transaction = context.BeginTransactionedContext();

            var user = await transaction.Users.OneById(userId);
            if (user == null) return false;

            var libraryCount = await context.Libraries.Count(l => libraryIds.Contains(l.Id));
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
            var container = ResolveContainer.Default;

            Library library;

            await using (var context = container.Resolve<IReadNxplxContext>())
            await using (var transaction = context.BeginTransactionedContext())
            {
                foreach (var user in await transaction.Users.Many(u => u.LibraryAccessIds.Contains(libraryId)))
                {
                    user.LibraryAccessIds.Remove(libraryId);
                }
                await transaction.SaveChanges();
                
                library = await transaction.Libraries.OneById(libraryId);
                if (library == null) return false;

                transaction.Libraries.Remove(library);
                await transaction.SaveChanges();
            }

            container.Resolve<ILoggingService>().Info("Deleted library {Username}", library.Name);
            return true;
        }
        public static async Task<List<int>?> FindLibraryAccess(int userId)
        {
            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>();

            var user = await context.Users.OneById(userId);
            return user?.LibraryAccessIds;
        }
        public static async Task<AdminLibraryDto> CreateNewLibrary(string name, string path, string language, string kind)
        {
            var lib = new Library
            {
                Name = name,
                Path = path,
                Language = language,
                Kind = kind == "film" ? LibraryKind.Film : LibraryKind.Series
            };

            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>();
            await using var transaction = context.BeginTransactionedContext();

            transaction.Libraries.Add(lib);
            await transaction.SaveChanges();

            container.Resolve<ILoggingService>().Info("Created library {Name} with {Path}", lib.Name, lib.Path);
            return container.Resolve<IDtoMapper>().Map<Library, AdminLibraryDto>(lib);
        }
    }
}