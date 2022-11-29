using NJsonSchema;
using NJsonSchema.Generation;
using NxPlx.Application.Core.Options;

namespace NxPlx.ApplicationHost.Api.Extensions;

public static class ApiDocumentationServiceCollectionExtensions
{
    public static IApplicationBuilder UseApiDocumentation(this WebApplication app)
    {
        var apiDocumentationOptions = app.Services.GetRequiredService<ApiDocumentationOptions>();
        if (apiDocumentationOptions.Enabled)
        {
            app.UseOpenApi(options => options.Path = apiDocumentationOptions.PathPrefix);
        }

        return app;
    }

    public static IServiceCollection AddApiDocumentation(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddHttpContextAccessor()
            .AddTransient<ISchemaProcessor, MarkAsRequiredIfNonNullableSchemaProcessor>()
            .AddOpenApiDocument((settings, provider) =>
            {
                var schemaProcessors = provider.GetServices<ISchemaProcessor>();
                foreach (var schemaProcessor in schemaProcessors)
                {
                    settings.SchemaProcessors.Add(schemaProcessor);
                }
            });
    }
    private class MarkAsRequiredIfNonNullableSchemaProcessor : ISchemaProcessor
    {
        public void Process(SchemaProcessorContext context)
        {
            foreach (var (_, prop) in context.Schema.Properties)
            {
                if (!prop.IsNullable(SchemaType.OpenApi3))
                {
                    prop.IsRequired = true;
                }
            }
        }
    }
}