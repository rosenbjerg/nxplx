using System;
using Autofac;

namespace NxPlx.Infrastructure.IoC
{
    public class ResolveContainer : IDisposable
    {
        public TInterface Resolve<TInterface>()
        {
            return ContainerManager.Default.Value.Resolve<TInterface>();
        }

        public void Dispose()
        {
            
        }
    }
}