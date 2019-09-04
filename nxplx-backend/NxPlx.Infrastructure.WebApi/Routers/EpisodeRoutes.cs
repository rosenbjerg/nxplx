using System.IO;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NxPlx.Models.Dto;
using NxPlx.Services.Database;
using Red.Interfaces;

namespace NxPlx.WebApi.Routers
{
    public static class EpisodeRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/:series_id", async (req, res) =>
            {
                var id = int.Parse(req.Context.ExtractUrlParameter("series_id"));
                
                using var ctx = new MediaContext();
                var seriesDetails = await ctx.Series.FindAsync(id);
                var episodes = await ctx.EpisodeFiles
                    .Where(e => e.SeriesDetailsId == id)
                    .ToListAsync();
                    
                if (seriesDetails == null)
                {
                    return await res.SendStatus(HttpStatusCode.NotFound);
                }
                    
                return await res.SendJson(new SeriesDto(seriesDetails, episodes));
            });
            
            router.Get("/:series_id/:season_no", async (req, res) =>
            {
                var id = int.Parse(req.Context.ExtractUrlParameter("series_id"));
                var no = int.Parse(req.Context.ExtractUrlParameter("season_no"));
                
                using var ctx = new MediaContext();
                var series = await ctx.Series.FindAsync(id);
                if (series == null)
                {
                    return await res.SendStatus(HttpStatusCode.NotFound);
                }
                    
                var season = series.Seasons.FirstOrDefault(s => s.SeasonDetails.SeasonNumber == no);
                if (season == null)
                {
                    return await res.SendStatus(HttpStatusCode.NotFound);
                }

                var seasonDetails = season.SeasonDetails;

                var episodes = await ctx.EpisodeFiles
                    .Where(e => e.SeriesDetailsId == id && e.SeasonNumber == no)
                    .ToListAsync();
                    
                return await res.SendJson(new { seasonDetails, episodes });
            });
            
            router.Get("/watch/:file_id", async (req, res) =>
            {
                var id = int.Parse(req.Context.ExtractUrlParameter("uuid"));
                using (var ctx = new MediaContext())
                {
                    var episode = await ctx.EpisodeFiles.FindAsync(id);
                    if (episode == null)
                    {
                        return await res.SendStatus(HttpStatusCode.NotFound);
                    }

                    return await res.SendFile(episode.Path);
                    
                    using (var mediaStream = File.Open(episode.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return await res.SendStream(mediaStream, "video/mp4");
                    }
                }
            });
        }
    }
}