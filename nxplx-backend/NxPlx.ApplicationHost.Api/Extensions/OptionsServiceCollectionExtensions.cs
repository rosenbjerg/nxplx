using Microsoft.Extensions.Options;
using NxPlx.Application.Core.Options;

namespace NxPlx.ApplicationHost.Api.Extensions;

public static class OptionsServiceCollectionExtensions
{
    public static IServiceCollection AddNxPlxOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class, INxplxOptions, new()
    {
        services.AddOptions<TOptions>()
            .Bind(configuration.GetSection(typeof(TOptions).Name.Replace("Options", string.Empty)))
            .ValidateDataAnnotations();

        return services
            .AddSingleton(typeof(TOptions), provider => provider.GetRequiredService<IOptions<TOptions>>().Value);
    }
}