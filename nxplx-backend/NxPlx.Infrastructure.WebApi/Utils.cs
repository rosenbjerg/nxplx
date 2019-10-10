using System.Collections.Generic;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using Red;

namespace NxPlx.WebApi
{
    public static class Utils
    {
        public static Task<HandlerType> SendMapped<TFrom, TTo>(this Response res, IMapper mapper, IEnumerable<TFrom> dtos)
            where TFrom : class
        {
            return res.SendJson(mapper.MapMany<TFrom, TTo>(dtos));
        }
        
        public static Task<HandlerType> SendMapped<TFrom, TTo>(this Response res, IMapper mapper, TFrom dto)
            where TFrom : class
        {
            return res.SendJson(mapper.Map<TFrom, TTo>(dto));
        }
    }
}