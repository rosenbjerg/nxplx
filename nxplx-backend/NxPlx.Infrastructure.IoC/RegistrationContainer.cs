using System;
using Autofac;

namespace NxPlx.Infrastructure.IoC
{
    public class RegistrationContainer
    {
        public void Register<TInterface, TInstance>(TInstance instance)
            where TInstance : class, TInterface
        {
            if (ContainerManager.Default.IsValueCreated) 
                throw new InvalidOperationException("Registration must take place before calls to resolve");
            
            ContainerManager.DefaultBuilder.Value.RegisterInstance(instance).As<TInterface>().SingleInstance();
        }
        
        public void Register<TInterface, TInstance>()
            where TInstance : class, TInterface
        {
            if (ContainerManager.Default.IsValueCreated) 
                throw new InvalidOperationException("Registration must take place before calls to resolve");

            ContainerManager.DefaultBuilder.Value.RegisterType<TInstance>().As<TInterface>().SingleInstance();
        }
        
//        public void Register<TInterface>()
//        {
//            if (ContainerManager.Default.IsValueCreated) 
//                throw new InvalidOperationException("Registration must take place before calls to resolve");
//
//            ContainerManager.DefaultBuilder.Value.RegisterType<TInterface>();
//        }
    }
}