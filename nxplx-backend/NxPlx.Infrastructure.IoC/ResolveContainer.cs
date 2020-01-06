using System.Linq;
using Autofac;

namespace NxPlx.Infrastructure.IoC
{
    public class ResolveContainer
    {
        private ResolveContainer()
        {
            
        }
        public TInterface Resolve<TInterface>(params object[] parameters)
        {
            if (parameters.Any())
            {
                var arguments = parameters.Select((parameter, index) => new PositionalParameter(index, parameter));
                return ContainerManager.Default.Value.Resolve<TInterface>(arguments);
            }
            return ContainerManager.Default.Value.Resolve<TInterface>();
        }

        private static readonly ResolveContainer _default = new ResolveContainer();

        public static ResolveContainer Default => _default;
    }
}