using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Models;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services
{
    public class LibraryService
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<LibraryService> _systemLogger;
        private readonly IDtoMapper _dtoMapper;

        public LibraryService(DatabaseContext context, ILogger<LibraryService> systemLogger, IDtoMapper dtoMapper)
        {
            _context = context;
            _systemLogger = systemLogger;
            _dtoMapper = dtoMapper;
        }
        public async Task<bool> SetLibraryAccess(int userId, List<int> libraryIds)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            var libraryCount = await _context.Libraries.Where(l => libraryIds.Contains(l.Id)).CountAsync();
            if (libraryIds.Count != libraryCount) return false;

            user.LibraryAccessIds = libraryIds;
            await _context.SaveChangesAsync();

            _systemLogger.LogInformation("Updated library permissions for user {Username}: {Permissions}", user.Username, string.Join(' ', user.LibraryAccessIds));
            return true;
        }


        public IEnumerable<string?> GetDirectoryEntries(string cwd)
        {
            if (cwd == string.Empty || !Directory.Exists(cwd))
                return Enumerable.Empty<string>();

            return Directory.EnumerateDirectories(cwd.Replace("\\", "/"), "*", new EnumerationOptions
            {
                AttributesToSkip = FileAttributes.Hidden | FileAttributes.Temporary | FileAttributes.System
            }).Select(path => Path.GetFileName(path).Replace("\\", "/"));
        }
        public async Task<bool> RemoveLibrary(int libraryId)
        {
            foreach (var user in await _context.Users.Where(u => u.LibraryAccessIds.Contains(libraryId)).ToListAsync())
            {
                user.LibraryAccessIds.Remove(libraryId);
            }
            await _context.SaveChangesAsync();

            var library = await _context.Libraries.FirstOrDefaultAsync(l => l.Id == libraryId);
            if (library == null) return false;

            _context.Libraries.Remove(library);
            await _context.SaveChangesAsync();

            _systemLogger.LogInformation("Deleted library {Username}", library.Name);
            return true;
        }
        public async Task<IEnumerable<TLibraryDto>> ListLibraries<TLibraryDto>()
            where TLibraryDto : LibraryDto
        {
            var libraries = await _context.Libraries.ToListAsync();
            return _dtoMapper.Map<Library, TLibraryDto>(libraries);
        } 
        public async Task<List<int>?> GetLibraryAccess(int userId)
        {
            return await _context.Users.Where(u => u.Id == userId).Select(u => u.LibraryAccessIds).FirstOrDefaultAsync();
        }
        public async Task<AdminLibraryDto> CreateNewLibrary(string name, string path, string language, string kind)
        {
            var lib = new Library
            {
                Name = name,
                Path = path,
                Language = language,
                Kind = kind == "film" ? LibraryKind.Film : LibraryKind.Series
            };

            _context.Libraries.Add(lib);
            await _context.SaveChangesAsync();

            _systemLogger.LogInformation("Created library {Name} with {Path}", lib.Name, lib.Path);
            return _dtoMapper.Map<Library, AdminLibraryDto>(lib)!;
        }
    }
}