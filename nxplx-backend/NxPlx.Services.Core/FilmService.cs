using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.Dto.Models.Film;
using NxPlx.Models.File;

namespace NxPlx.Core.Services
{
    public static class FilmService
    {
        public static async Task<FilmDto?> FindFilmByDetails(int id, User user)
        {
            var container = ResolveContainer.Default;
            await using var ctx = container.Resolve<IReadNxplxContext>(user);
            var filmFile = await ctx.FilmFiles.One(ff => ff.FilmDetailsId == id);

            return container.Resolve<IDtoMapper>().Map<FilmFile, FilmDto>(filmFile);
        }
        public static async Task<MovieCollectionDto> FindCollectionByDetails(int id, User user)
        {
            var container = ResolveContainer.Default;
            await using var ctx = container.Resolve<IReadNxplxContext>(user);
            var filmFiles = await ctx.FilmFiles.Many(ff => ff.FilmDetails.BelongsInCollectionId == id, ff => ff.FilmDetails).ToListAsync();

            var mapper = container.Resolve<IDtoMapper>();
            var collection = mapper.Map<MovieCollection, MovieCollectionDto>(filmFiles.First().FilmDetails.BelongsInCollection);
            collection!.movies = mapper.Map<DbFilmDetails, OverviewElementDto>(filmFiles.Select(ff => ff.FilmDetails)).ToList();
            
            return collection;
        }
        public static async Task<InfoDto?> FindFilmFileInfo(int fileId, User user)
        {
            var container = ResolveContainer.Default;

            await using var ctx = container.Resolve<IReadNxplxContext>(user);
            var filmFile = await ctx.FilmFiles.One(ff => ff.Id == fileId);
            return container.Resolve<IDtoMapper>().Map<FilmFile, InfoDto>(filmFile);
            
        }
        public static async Task<string> FindFilmFilePath(int fileId, User user)
        {
            await using var ctx = ResolveContainer.Default.Resolve<IReadNxplxContext>(user);
            return await ctx.FilmFiles.ProjectOne(ff => ff.Id == fileId, ff => ff.Path);
        }
    }
}