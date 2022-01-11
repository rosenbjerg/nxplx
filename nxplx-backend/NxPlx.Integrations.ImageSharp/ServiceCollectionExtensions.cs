using Microsoft.Extensions.DependencyInjection;
using NxPlx.Application.Services;

namespace NxPlx.Integrations.ImageSharp;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddImageSharpImageService(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IImageService, ImageSharpImageService>();
    }
}