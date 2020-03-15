using System.IO;
using System.Net;
using System.Threading.Tasks;
using NxPlx.Core.Services;
using NxPlx.Infrastructure.Session;
using Red;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class EpisodeRoutes
    {
        public static void BindHandlers(IRouter router)
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
            var fileId = int.Parse(req.Context.Params["file_id"]!);
            
            var next = await EpisodeService.TryFindNextEpisode(fileId, session!.User);
            return await res.SendJson(next);
        }

        private static async Task<HandlerType> StreamFile(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.Params["file_id"]!);
            
            var episodePath = await EpisodeService.FindEpisodeFilePath(id, session!.User);

            if (episodePath == null || !File.Exists(episodePath)) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendFile(episodePath, "video/mp4");
        }
        
        private static async Task<HandlerType> GetFileInfo(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.Params["file_id"]!);

            var episode = await EpisodeService.FindEpisodeFileInfo(id, session!.User);

            if (episode == null) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendJson(episode);
        }
        
        private static async Task<HandlerType> GetSeasonDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.Params["series_id"]!);
            var no = int.Parse(req.Context.Params["season_no"]!);
            
            var seriesDto = await EpisodeService.FindSeriesDetails(id, no, session!.User);
            
            if (seriesDto == null) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendJson(seriesDto);
        }

        private static async Task<HandlerType> GetSeriesDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.Params["series_id"]!);

            var seriesDto = await EpisodeService.FindSeriesDetails(id, null, session!.User);

            if (seriesDto == null) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendJson(seriesDto);
        }

      
    }
}