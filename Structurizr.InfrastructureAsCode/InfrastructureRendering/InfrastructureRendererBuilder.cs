using System;
using System.Linq;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;
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
            var resolverTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IConfigurationValueResolver).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition);

            foreach (var resolverType in resolverTypes)
            {
                foreach (var resolverInterface in resolverType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConfigurationValueResolver<>)))
                {
                    Ioc.Register(resolverInterface, resolverType).AsMultiInstance();
                }
            }

            Ioc.Register<IConfigurationValueResolver<ConfigurationValue<string>>, FixedConfigurationValueResolver<string>>().AsMultiInstance();
            Ioc.Register<IConfigurationValueResolver<ConfigurationValue<int>>, FixedConfigurationValueResolver<int>>().AsMultiInstance();
        }

        public TBuilder In(TEnvironment environment)
        {
            Ioc.Register(environment);
            return (TBuilder)this;
        }

        public TBuilder UsingConfigurationValueResolver<TValue, TResolver>()
            where TValue : ConfigurationValue
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

        public TRenderer Build()
        {
            return Ioc.Resolve<TRenderer>();
        }
    }
}