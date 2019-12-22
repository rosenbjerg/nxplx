using System.IO;
using System.Net;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Session;
using NxPlx.Infrastructure.WebApi.Routes.Services;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.File;
using Red;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class EpisodeRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/detail/:series_id", Authenticated.User, GetSeriesDetails);
            router.Get("/detail/:series_id/:season_no", Authenticated.User, GetSeasonDetails);
            
            router.Get("/info/:file_id", Authenticated.User, GetFileInfo);
            router.Get("/watch/:file_id", Authenticated.User, StreamFile);
            router.Get("/next/:file_id", Authenticated.User, GetNext);
        }

        private static async Task<HandlerType> GetNext(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            
            var fileId = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            
            var next = await EpisodeService.TryFindNextEpisode(fileId, session.IsAdmin, session.User.LibraryAccessIds);

            return await res.SendDto<EpisodeFile, NextEpisodeDto>(next);
        }

        private static async Task<HandlerType> StreamFile(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            
            var episode = await EpisodeService.FindEpisodeFile(id, session.IsAdmin, session.User.LibraryAccessIds);

            if (episode == null || !File.Exists(episode.Path)) 
                return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendFile(episode.Path);
        }
        
        private static async Task<HandlerType> GetFileInfo(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));

            var episode = await EpisodeService.FindFileInfo(id, session.IsAdmin, session.User.LibraryAccessIds);

            if (episode == null) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendDto<EpisodeFile, InfoDto>(episode);
        }
        
        private static async Task<HandlerType> GetSeasonDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("series_id"));
            var no = int.Parse(req.Context.ExtractUrlParameter("season_no"));
            
            var seriesDto = await EpisodeService.FindSeriesDetails(id, session.IsAdmin, session.User.LibraryAccessIds, no);
            
            if (seriesDto == null) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendJson(seriesDto);
        }

        private static async Task<HandlerType> GetSeriesDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("series_id"));

            var seriesDto = await EpisodeService.FindSeriesDetails(id, session.IsAdmin, session.User.LibraryAccessIds, null);

            if (seriesDto == null) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendJson(seriesDto);
        }

      
    }
}