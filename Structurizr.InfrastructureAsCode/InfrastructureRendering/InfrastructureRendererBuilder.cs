using System;
using System.Linq;
using System.Reflection;
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
            try
            {
                var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a =>
                    {
                        try
                        {
                            return a.GetTypes();
                        }
                        catch
                        {
                            return Enumerable.Empty<Type>();
                        }
                    })
                    .ToArray();

                foreach (var injectable in allTypes.Where(t => t.GetCustomAttribute(typeof(InjectableAttribute)) != null))
                {
                    var att = injectable.GetCustomAttributes(typeof(InjectableAttribute))
                        .OfType<InjectableAttribute>()
                        .Single();
                    foreach (var injectableInterface in injectable.GetInterfaces())
                    {
                        if (att.Singleton)
                        {
                            Ioc.Register(injectableInterface, injectable).AsSingleton();
                        }
                        else
                        {
                            Ioc.Register(injectableInterface, injectable).AsMultiInstance();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public TBuilder In(TEnvironment environment)
        {
            Ioc.Register(environment);
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