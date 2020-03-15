using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using NxPlx.Models.Dto.Models;

namespace NxPlx.Core.Services
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

            var libraryCount = await context.Libraries.Many(l => libraryIds.Contains(l.Id)).CountAsync();
            if (libraryIds.Count != libraryCount) return false;

            user.LibraryAccessIds = libraryIds;
            await transaction.SaveChanges();

            container.Resolve<ILoggingService>()
                .Info("Updated library permissions for user {Username}: {Permissions}", user.Username, string.Join(' ', user.LibraryAccessIds));
            return true;
        }


        public static IEnumerable<string?> GetDirectoryEntries(string cwd)
        {
            if (cwd == string.Empty || !Directory.Exists(cwd))
                return Enumerable.Empty<string>();

            return Directory.EnumerateDirectories(cwd, "*", new EnumerationOptions
            {
                AttributesToSkip = FileAttributes.Hidden | FileAttributes.Temporary | FileAttributes.System
            }).Select(Path.GetFileName);
        }
        public static async Task<bool> RemoveLibrary(int libraryId)
        {
            var container = ResolveContainer.Default;

            await using var context = container.Resolve<IReadNxplxContext>();
            await using var transaction = context.BeginTransactionedContext();
            foreach (var user in await transaction.Users.Many(u => u.LibraryAccessIds.Contains(libraryId)).ToListAsync())
            {
                user.LibraryAccessIds.Remove(libraryId);
            }
            await transaction.SaveChanges();
                
            var library = await transaction.Libraries.OneById(libraryId);
            if (library == null) return false;

            transaction.Libraries.Remove(library);
            await transaction.SaveChanges();

            container.Resolve<ILoggingService>().Info("Deleted library {Username}", library.Name);
            return true;
        }
        public static async Task<IEnumerable<LibraryDto>> ListLibraries(User user)
        {
            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>(user);

            var libraries = await context.Libraries.Many().ToListAsync();
            return container.Resolve<IDtoMapper>().Map<Library, AdminLibraryDto>(libraries);
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
            return container.Resolve<IDtoMapper>().Map<Library, AdminLibraryDto>(lib)!;
        }
    }
}