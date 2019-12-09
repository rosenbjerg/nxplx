using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using Red;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class Utils
    {
        public static Task<HandlerType> SendMapped<TFrom, TTo>(this Response res, IMapper mapper, IEnumerable<TFrom> dtos)
            where TFrom : class
        {
            return res.SendJson(mapper.MapMany<TFrom, TTo>(dtos));
        }
        
        public static Task<HandlerType> SendMapped<TFrom, TTo>(this Response res, IMapper mapper, TFrom dto)
            where TTo : class
            where TFrom : class
        {
            return res.SendJson(mapper.Map<TFrom, TTo>(dto));
        }

        public static Task<HandlerType> SendSPA(Request req, Response res)
        {
            return res.SendFile(Path.Combine("public", "index.html"));
        }

    }
}