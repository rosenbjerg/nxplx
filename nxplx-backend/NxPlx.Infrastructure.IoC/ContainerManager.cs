using System;
using Autofac;

namespace NxPlx.Infrastructure.IoC
{
    static class ContainerManager
    {
        internal static Lazy<ContainerBuilder> DefaultBuilder =  new Lazy<ContainerBuilder>();
        internal static Lazy<IContainer> Default = new Lazy<IContainer>(() => DefaultBuilder.Value.Build()); 
    }
}