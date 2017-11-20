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
        void Render(AzureDeploymentTemplate template, IHaveInfrastructure elementWithInfrastructure, IAzureInfrastructureEnvironment environment, string resourceGroup, string location);
        IEnumerable<IConfigurationValue> GetConfigurationValues(IHaveInfrastructure elementWithInfrastructure);
        Task Configure(IHaveInfrastructure elementWithInfrastructure, AzureConfigurationValueResolverContext context);
    }

    public abstract class AzureResourceRenderer<TInfrastructure> : IAzureResourceRenderer
        where TInfrastructure : ContainerInfrastructure
    {
        public void Render(AzureDeploymentTemplate template, IHaveInfrastructure elementWithInfrastructure, IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            Render(template, (IHaveInfrastructure<TInfrastructure>) elementWithInfrastructure, environment, resourceGroup, location);
        }

        public IEnumerable<IConfigurationValue> GetConfigurationValues(IHaveInfrastructure elementWithInfrastructure)
        {
            return GetConfigurationValues((IHaveInfrastructure<TInfrastructure>)elementWithInfrastructure);
        }

        public Task Configure(IHaveInfrastructure elementWithInfrastructure, AzureConfigurationValueResolverContext context)
        {
            return Configure((IHaveInfrastructure<TInfrastructure>)elementWithInfrastructure, context);
        }

        protected virtual IEnumerable<IConfigurationValue> GetConfigurationValues(IHaveInfrastructure<TInfrastructure> elementWithInfrastructure)
        {
            return Enumerable.Empty<IConfigurationValue>();
        }

        protected virtual Task Configure(IHaveInfrastructure<TInfrastructure> elementWithInfrastructure, AzureConfigurationValueResolverContext context)
        {
            return Task.FromResult(0);
        }

        protected abstract void Render(AzureDeploymentTemplate template, IHaveInfrastructure<TInfrastructure> elementWithInfrastructure, IAzureInfrastructureEnvironment environment, string resourceGroup, string location);

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

        protected JObject Template(string type, string name, string location, string apiVersion = "2015-05-01")
        {
            var template = new JObject
            {
                ["type"] = type,
                ["name"] = name,
                ["apiVersion"] = apiVersion,
                ["location"] = ToLocationName(location)
            };
            
            return template;
        }
    }

    public static class AzureResourceRendererExtensions
    {
        public static IAzureResourceRenderer GetRendererFor(this TinyIoCContainer ioc, IHaveInfrastructure elementWithInfrastructure)        {
            var resolverType = typeof(AzureResourceRenderer<>).MakeGenericType(elementWithInfrastructure.Infrastructure.GetType());
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
