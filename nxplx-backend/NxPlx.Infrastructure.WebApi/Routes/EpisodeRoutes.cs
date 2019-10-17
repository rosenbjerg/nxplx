using System.Collections.Generic;
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

namespace NxPlx.WebApi.Routes
{
    public static class EpisodeRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/detail/:series_id", Authenticated.User, GetSeriesDetails);
            router.Get("/detail/:series_id/:season_no", Authenticated.User, GetSeasonDetails);
            
            router.Get("/info/:file_id", Authenticated.User, GetFileInfo);
            router.Get("/watch/:file_id", Authenticated.User, StreamFile);
        }

        private static async Task<HandlerType> StreamFile(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id").Replace(".mp4", ""));
            await using var ctx = new MediaContext();
            
            var episode = await ctx.EpisodeFiles
                .Where(ef => ef.Id == id && (session.IsAdmin || session.LibraryAccess.Contains(ef.PartOfLibraryId)))
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

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<MediaContext>();

            var episode = await ctx.EpisodeFiles
                .Where(ef => ef.Id == id && (session.IsAdmin || session.LibraryAccess.Contains(ef.PartOfLibraryId)))
                .FirstOrDefaultAsync();

            return await res.SendMapped<EpisodeFile, InfoDto>(container.Resolve<IDatabaseMapper>(), episode);
        }
        
        private static async Task<HandlerType> GetSeasonDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("series_id"));
            var no = int.Parse(req.Context.ExtractUrlParameter("season_no"));

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<MediaContext>();

            var episodes = await ctx.EpisodeFiles
                .Where(ef =>
                    ef.SeriesDetailsId == id && ef.SeasonNumber == no &&
                    (session.IsAdmin || session.LibraryAccess.Contains(ef.PartOfLibraryId)))
                .ToListAsync();
            
            if (!episodes.Any())
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            var dtoMapper = container.Resolve<IDatabaseMapper>();
            var seriesDetails = episodes.First().SeriesDetails;

            return await res.SendJson(MergeEpisodes(dtoMapper, seriesDetails, episodes));
        }

        private static async Task<HandlerType> GetSeriesDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("series_id"));

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<MediaContext>();
            
            var episodes = await ctx.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId == id && (session.IsAdmin || session.LibraryAccess.Contains(ef.PartOfLibraryId)))
                .ToListAsync();
            
            if (!episodes.Any())
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            var dtoMapper = container.Resolve<IDatabaseMapper>();
            var seriesDetails = episodes.First().SeriesDetails;

            return await res.SendJson(MergeEpisodes(dtoMapper, seriesDetails, episodes));
        }
        
        private static SeriesDto MergeEpisodes(IMapper mapper, DbSeriesDetails series, IReadOnlyCollection<EpisodeFile> files)
        {
            var seriesDto = mapper.Map<DbSeriesDetails, SeriesDto>(series);
            seriesDto.seasons = series.Seasons
                .Select(s => MergeEpisodes(mapper, s, files.Where(f => f.SeasonNumber == s.SeasonNumber)))
                .Where(s => s.episodes.Any());
            return seriesDto;
        }
        private static SeasonDto MergeEpisodes(IMapper mapper, SeasonDetails seasonDetails, IEnumerable<EpisodeFile> files)
        {
            var seasonDto = mapper.Map<SeasonDetails, SeasonDto>(seasonDetails);
            seasonDto.episodes = MergeEpisodes(seasonDetails.Episodes, files);
            return seasonDto;
        }
        private static IEnumerable<EpisodeDto> MergeEpisodes(IEnumerable<EpisodeDetails> details, IEnumerable<EpisodeFile> files)
        {
            var mapping = details.ToDictionary(d => (d.SeasonNumber, d.EpisodeNumber));
            return files.Select(f =>
            {
                if (!mapping.TryGetValue((f.SeasonNumber, f.EpisodeNumber), out var details))
                {
                    details = new EpisodeDetails
                    {
                        Name = "",
                        StillPath = "notfound.jpg"
                    };
                }

                return new EpisodeDto
                {
                    name = details.Name,
                    airDate = details.AirDate,
                    number = f.EpisodeNumber,
                    fileId = f.Id,
                    still = details.StillPath,
                };
            });
        }
    }
}