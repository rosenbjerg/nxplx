using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Core.Validation;
using Red;
using Validation;

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
            where TTo : class
            where TFrom : class
        {
            return res.SendJson(mapper.Map<TFrom, TTo>(dto));
        }

        public static Func<Request, Response, Task<HandlerType>> Validate(Forms formEnum)
        {
            return async (req, res) =>
            {
                var validator = Validators.Select(formEnum);
                var form = await req.GetFormDataAsync();
                if (!validator.Validate(form))
                {
                    return await res.SendStatus(HttpStatusCode.BadRequest);
                }

                return HandlerType.Continue;
            };
        }

        public static Task<HandlerType> SendSPA(Request req, Response res)
        {
            return res.SendFile(Path.Combine("public", "index.html"));
        }

    }
}