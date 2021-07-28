using System;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Infrastructure.Events
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventHandlingFramework(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IEventDispatcher, EventDispatcher>()
                .AddScoped<ICachingEventDispatcher, CachingApplicationEventDispatcher>()
                .AddScoped<IApplicationEventDispatcher, ApplicationEventDispatcher>()
                .AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        }

        public static IServiceCollection AddApplicationEventHandlers(this IServiceCollection serviceCollection,
            Type assemblyMarker)
        {
            return serviceCollection.Scan(scan => scan
                .FromAssembliesOf(assemblyMarker)
                .AddClasses(classes => classes.AssignableTo<IApplicationEventHandler>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }

        public static IServiceCollection AddDomainEventHandlers(this IServiceCollection serviceCollection,
            Type assemblyMarker)
        {
            return serviceCollection.Scan(scan => scan
                .FromAssembliesOf(assemblyMarker)
                .AddClasses(classes => classes.AssignableTo<IDomainEventHandler>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }
    }
}