using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.ARM.Configuration;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public interface IAzureResourceRenderer
    {
        IEnumerable<JObject> Render(Container container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location);
        IEnumerable<ConfigurationValue> GetConfigurationValues(Container container);
        Task Configure(Container container, AzureConfigurationValueResolverContext context);
    }

    public abstract class AzureResourceRenderer<TInfrastructure> : IAzureResourceRenderer
        where TInfrastructure : ContainerInfrastructure
    {
        public IEnumerable<JObject> Render(Container container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            return Render((Container<TInfrastructure>) container, environment, resourceGroup, location);
        }

        public IEnumerable<ConfigurationValue> GetConfigurationValues(Container container)
        {
            return GetConfigurationValues((Container<TInfrastructure>)container);
        }

        public Task Configure(Container container, AzureConfigurationValueResolverContext context)
        {
            return Configure((Container<TInfrastructure>)container, context);
        }

        protected virtual IEnumerable<ConfigurationValue> GetConfigurationValues(
            Container<TInfrastructure> container)
        {
            return Enumerable.Empty<ConfigurationValue>();
        }

        protected virtual Task Configure(Container<TInfrastructure> container, AzureConfigurationValueResolverContext context)
        {
            return Task.FromResult(0);
        }

        protected abstract IEnumerable<JObject> Render(Container<TInfrastructure> container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location);

        protected string ToLocationName(string location)
        {
            switch (location)
            {
                case "westeurope":
                    return "West Europe";
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public static class AzureResourceRendererExtensions
    {
        public static IAzureResourceRenderer GetRendererFor(this TinyIoC.TinyIoCContainer ioc, Container container)        {
            var resolverType = typeof(AzureResourceRenderer<>).MakeGenericType(container.Infrastructure.GetType());
            var resolver = ioc.Resolve(resolverType);
            return (IAzureResourceRenderer)resolver;
        }
    }
}
