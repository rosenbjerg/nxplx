using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NxPlx.Application.Core.Options;

namespace NxPlx.ApplicationHost.Api
{
    public static class ApiDocumentationExtensions
    {
        public static IServiceCollection AddApiDocumentation(this IServiceCollection serviceCollection, ApiDocumentationOptions apiDocumentationOptions)
        {
            if (apiDocumentationOptions.Enabled) 
                serviceCollection.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "NxPlx API", Version = "v1" }));
            return serviceCollection;
        }

        public static void UseApiDocumentation(this IApplicationBuilder app, string documentationUrl, ApiDocumentationOptions apiDocumentationOptions)
        {
            if (apiDocumentationOptions.Enabled)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint($"${documentationUrl}/v1/swagger.json", "NxPlx API"));
            }
        }
    }
}