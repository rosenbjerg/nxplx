using System;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NxPlx.Models;
using NxPlx.Models.File;
using NxPlx.Services.Database;
using Red.Interfaces;

namespace NxPlx.WebApi.Routers
{
    public static class EpisodeRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/:uuid", async (req, res) =>
            {
                var uuid = Guid.Parse(req.Context.ExtractUrlParameter("uuid"));
                using (var ctx = new MediaContext())
                {
                    var episode = await ctx.Episodes
                        .Include(e => e.Season)
                        .ThenInclude(s => s.Series)
                        .FirstOrDefaultAsync(e => e.Id == uuid);
                    
                    if (episode == null)
                    {
                        return await res.SendStatus(HttpStatusCode.NotFound);
                    }
                    
                    return await res.SendJson(episode);
                }
            });
            
            router.Get("/:uuid/details", async (req, res) =>
            {
                var uuid = Guid.Parse(req.Context.ExtractUrlParameter("uuid"));
                using (var ctx = new MediaContext())
                {
                    var episodeFile = await ctx.EpisodeFiles
                        .Include(e => e.Subtitles)
                        .Include(e => e.MediaDetails)
                        .FirstOrDefaultAsync(e => e.Id == uuid);

                    var episode = await ctx.Episodes.FindAsync(uuid);
                    
                    if (episodeFile == null)
                    {
                        return await res.SendStatus(HttpStatusCode.NotFound);
                    }
                    
                    return await res.SendJson(episodeFile);
                }
            });
            
            router.Get("/:uuid/watch", async (req, res) =>
            {
                var uuid = Guid.Parse(req.Context.ExtractUrlParameter("uuid"));
                using (var ctx = new MediaContext())
                {
                    var episode = await ctx.EpisodeFiles.FindAsync(uuid);
                    if (episode == null)
                    {
                        return await res.SendStatus(HttpStatusCode.NotFound);
                    }

                    using (var mediaStream = File.Open(episode.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return await res.SendStream(mediaStream, "video/mp4");
                    }
                }
            });
            
//            router.Get("/:eid/watch", fun (req:Request) (res:Response) ->
//                let eid = req.Context.ExtractUrlParameter "eid" |> Int32.Parse
//            let episode = Map.find eid indexer.episodeEntries
//            res.SendFile (episode.path, "video/mp4", true))
        }
    }
}