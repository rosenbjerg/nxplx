using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Series;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.Dto.Models.Series;
using NxPlx.Models.File;
using NxPlx.Services.Database;
using Red;
using Red.Interfaces;

namespace NxPlx.WebApi.Routers
{
    public static class EpisodeRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/detail/:series_id", Authenticated.User, GetSeriesDetails);
            router.Get("/detail/:series_id/:season_no", Authenticated.User, GetSeasonDetails);
            
            router.Get("/episodes/:series_id", Authenticated.User, GetEpisodes);
            router.Get("/episodes/:series_id/:season_no", Authenticated.User, GetEpisodesBySeason);
            
            router.Get("/info/:file_id", Authenticated.User, GetFileInfo);
            router.Get("/watch/:file_id", Authenticated.User, StreamFile);
        }

        private static async Task<HandlerType> StreamFile(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            await using var ctx = new MediaContext();
            
            var episode = await ctx.EpisodeFiles
                .Where(ef => ef.Id == id && session.LibraryAccess.Contains(ef.PartOfLibraryId))
                .FirstOrDefaultAsync();
            
            if (episode == null || !File.Exists(episode.Path))
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendFile(episode.Path);
        }

        private static async Task<HandlerType> GetFileInfo(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));

            var container = new ResolveContainer();
            await using var ctx = container.Resolve<MediaContext>();
            var dtoMapper = container.Resolve<IDatabaseMapper>();

            var episode = await ctx.EpisodeFiles
                .Where(ef => ef.Id == id && session.LibraryAccess.Contains(ef.PartOfLibraryId))
                .FirstOrDefaultAsync();

            return await res.SendJson(dtoMapper.Map<EpisodeFile, InfoDto>(episode));
        }

        private static async Task<HandlerType> GetEpisodesBySeason(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("series_id"));
            var no = int.Parse(req.Context.ExtractUrlParameter("season_no"));

            var container = new ResolveContainer();
            await using var ctx = container.Resolve<MediaContext>();
            var dtoMapper = container.Resolve<IDatabaseMapper>();

            var episodes = await ctx.EpisodeFiles
                .Where(ef =>
                    ef.SeriesDetailsId == id && ef.SeasonNumber == no &&
                    session.LibraryAccess.Contains(ef.PartOfLibraryId))
                .ToListAsync();
            
            return await res.SendJson(dtoMapper.MapMany<EpisodeFile, EpisodeFileDto>(episodes));
        }

        private static async Task<HandlerType> GetEpisodes(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("series_id"));

            var container = new ResolveContainer();
            await using var ctx = container.Resolve<MediaContext>();
            var dtoMapper = container.Resolve<IDatabaseMapper>();

            var episodes = await ctx.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId == id && session.LibraryAccess.Contains(ef.PartOfLibraryId))
                .ToListAsync();

            return await res.SendJson(dtoMapper.MapMany<EpisodeFile, EpisodeFileDto>(episodes));
        }

        private static async Task<HandlerType> GetSeasonDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("series_id"));
            var no = int.Parse(req.Context.ExtractUrlParameter("season_no"));

            var container = new ResolveContainer();
            await using var ctx = container.Resolve<MediaContext>();
            var dtoMapper = container.Resolve<IDatabaseMapper>();

            var episodeFile = await ctx.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId == id && session.LibraryAccess.Contains(ef.PartOfLibraryId))
                .FirstOrDefaultAsync();
            
            if (episodeFile == null)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            var season = episodeFile.SeriesDetails.Seasons.FirstOrDefault(s => s.SeasonNumber == no);
            if (season == null)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendJson(dtoMapper.Map<SeasonDetails, SeasonDto>(season));
        }

        private static async Task<HandlerType> GetSeriesDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("series_id"));

            var container = new ResolveContainer();
            await using var ctx = container.Resolve<MediaContext>();
            var dtoMapper = container.Resolve<IDatabaseMapper>();
            
            var episodeFile = await ctx.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId == id && session.LibraryAccess.Contains(ef.PartOfLibraryId))
                .FirstOrDefaultAsync();
            
            if (episodeFile == null)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendJson(dtoMapper.Map<DbSeriesDetails, SeriesDto>(episodeFile.SeriesDetails));
        }
    }
}