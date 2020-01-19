using System.IO;
using System.Threading.Tasks;
using Red;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class Utils
    {
        // public static Task<HandlerType> SendDto<TFrom, TTo>(this Response res, IEnumerable<TFrom> dtos)
        //     where TTo : class, IDto
        //     where TFrom : class
        // {
        //     var mapper = ResolveContainer.Default().Resolve<IDtoMapper>();
        //     return res.SendJson(mapper.MapMany<TFrom, TTo>(dtos));
        // }
        //
        // public static Task<HandlerType> SendDto<TFrom, TTo>(this Response res, TFrom dto)
        //     where TTo : class, IDto
        //     where TFrom : class
        // {
        //     var mapper = ResolveContainer.Default().Resolve<IDtoMapper>();
        //     return res.SendJson(mapper.Map<TFrom, TTo>(dto));
        // }
        public static Task<HandlerType> SendSPA(Request req, Response res)
        {
            return res.SendFile(Path.Combine("public", "index.html"));
        }

    }
}