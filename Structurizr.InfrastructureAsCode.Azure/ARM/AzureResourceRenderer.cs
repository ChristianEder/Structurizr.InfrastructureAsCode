using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.ARM.Configuration;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using TinyIoC;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public interface IAzureResourceRenderer
    {
        IEnumerable<JObject> Render(ContainerWithInfrastructure container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location);
        IEnumerable<ConfigurationValue> GetConfigurationValues(ContainerWithInfrastructure container);
        Task Configure(ContainerWithInfrastructure container, AzureConfigurationValueResolverContext context);
    }

    public abstract class AzureResourceRenderer<TInfrastructure> : IAzureResourceRenderer
        where TInfrastructure : ContainerInfrastructure
    {
        public IEnumerable<JObject> Render(ContainerWithInfrastructure container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            return Render((ContainerWithInfrastructure<TInfrastructure>) container, environment, resourceGroup, location);
        }

        public IEnumerable<ConfigurationValue> GetConfigurationValues(ContainerWithInfrastructure container)
        {
            return GetConfigurationValues((ContainerWithInfrastructure<TInfrastructure>)container);
        }

        public Task Configure(ContainerWithInfrastructure container, AzureConfigurationValueResolverContext context)
        {
            return Configure((ContainerWithInfrastructure<TInfrastructure>)container, context);
        }

        protected virtual IEnumerable<ConfigurationValue> GetConfigurationValues(ContainerWithInfrastructure<TInfrastructure> container)
        {
            return Enumerable.Empty<ConfigurationValue>();
        }

        protected virtual Task Configure(ContainerWithInfrastructure<TInfrastructure> container, AzureConfigurationValueResolverContext context)
        {
            return Task.FromResult(0);
        }

        protected abstract IEnumerable<JObject> Render(ContainerWithInfrastructure<TInfrastructure> container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location);

        protected string ToLocationName(string location)
        {
            switch (location)
            {
                case "westeurope":
                    return "West Europe";
                case "northeurope":
                    return "North Europe";
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public static class AzureResourceRendererExtensions
    {
        public static IAzureResourceRenderer GetRendererFor(this TinyIoC.TinyIoCContainer ioc, ContainerWithInfrastructure container)        {
            var resolverType = typeof(AzureResourceRenderer<>).MakeGenericType(container.Infrastructure.GetType());
            try
            {
                var resolver = ioc.Resolve(resolverType);
                return (IAzureResourceRenderer)resolver;
            }
            catch (TinyIoCResolutionException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
