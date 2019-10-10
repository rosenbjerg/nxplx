using System;
using Autofac;

namespace NxPlx.Infrastructure.IoC
{
    public class ResolveContainer
    {
        private ResolveContainer()
        {
            
        }
        public TInterface Resolve<TInterface>()
        {
            return ContainerManager.Default.Value.Resolve<TInterface>();
        }

        private static readonly ResolveContainer _default = new ResolveContainer();
        
        public static ResolveContainer Default()
        {
            return _default;
        }
    }
}