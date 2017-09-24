using System;
using System.Linq;
using System.Reflection;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;
using Structurizr.InfrastructureAsCode.IoC;
using TinyIoC;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering
{
    public abstract class InfrastructureRendererBuilder<TBuilder, TRenderer, TEnvironment>
        where TRenderer : class, IInfrastructureRenderer
        where TEnvironment : class, IInfrastructureEnvironment
        where TBuilder : InfrastructureRendererBuilder<TBuilder, TRenderer, TEnvironment>
    {
        protected readonly TinyIoCContainer Ioc = new TinyIoCContainer();

        protected InfrastructureRendererBuilder()
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .ToArray();

            var resolverTypes = allTypes
                .Where(t => typeof(IConfigurationValueResolver).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition);

            foreach (var resolverType in resolverTypes)
            {
                foreach (var resolverInterface in resolverType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConfigurationValueResolver<>)))
                {
                    Ioc.Register(resolverInterface, resolverType).AsMultiInstance();
                }
            }

            Ioc.Register<IConfigurationValueResolver<FixedConfigurationValue<string>>, FixedConfigurationValueResolver<string>>().AsMultiInstance();
            Ioc.Register<IConfigurationValueResolver<FixedConfigurationValue<int>>, FixedConfigurationValueResolver<int>>().AsMultiInstance();

            foreach (var injectable in allTypes.Where(t => t.GetCustomAttribute(typeof(InjectableAttribute)) != null))
            {
                foreach (var injectableInterface in injectable.GetInterfaces())
                {
                    Ioc.Register(injectableInterface, injectable).AsMultiInstance();
                }
            }
        }

        public TBuilder In(TEnvironment environment)
        {
            Ioc.Register(environment);
            return (TBuilder)this;
        }

        public TBuilder UsingConfigurationValueResolver<TValue, TResolver>()
            where TValue : IConfigurationValue
            where TResolver : class, IConfigurationValueResolver<TValue>
        {
            Ioc.Register<IConfigurationValueResolver<TValue>, TResolver>().AsMultiInstance();
            return (TBuilder)this;
        }

        public TBuilder Using<TInterface, TImplementation>()
            where TImplementation : class, TInterface 
            where TInterface : class
        {
            Ioc.Register<TInterface, TImplementation>().AsMultiInstance();
            return (TBuilder)this;
        }

        public TBuilder Using<TInterface>(TInterface instance)
            where TInterface : class
        {
            Ioc.Register(instance);
            return (TBuilder)this;
        }

        public TRenderer Build()
        {
            return Ioc.Resolve<TRenderer>();
        }
    }
}