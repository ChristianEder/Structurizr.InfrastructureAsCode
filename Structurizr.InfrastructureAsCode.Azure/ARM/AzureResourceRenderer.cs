using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.ARM.Configuration;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public abstract class AzureResourceRenderer
    {
        public abstract bool CanRender(Container container);
        public abstract IEnumerable<JObject> Render(Container container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location);
        public abstract bool CanConfigure(Container container);
        public abstract IEnumerable<ContainerInfrastructureConfigurationElementValue> GetConfigurationValues(Container container);
        public abstract Task Configure(Container container, AzureContainerInfrastructureConfigurationElementValueResolverContext context);
    }

    public abstract class AzureResourceRenderer<TInfrastructure> : AzureResourceRenderer
        where TInfrastructure : ContainerInfrastructure
    {
        public override bool CanRender(Container container)
        {
            return container is Container<TInfrastructure>;
        }

        public override IEnumerable<JObject> Render(Container container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            return Render((Container<TInfrastructure>) container, environment, resourceGroup, location);
        }

        public override bool CanConfigure(Container container)
        {
            return container is Container<TInfrastructure>;
        }

        public override IEnumerable<ContainerInfrastructureConfigurationElementValue> GetConfigurationValues(Container container)
        {
            return GetConfigurationValues((Container<TInfrastructure>)container);
        }

        public override Task Configure(Container container, AzureContainerInfrastructureConfigurationElementValueResolverContext context)
        {
            return Configure((Container<TInfrastructure>)container, context);
        }

        protected virtual IEnumerable<ContainerInfrastructureConfigurationElementValue> GetConfigurationValues(
            Container<TInfrastructure> container)
        {
            return Enumerable.Empty<ContainerInfrastructureConfigurationElementValue>();
        }

        protected virtual Task Configure(Container<TInfrastructure> container, AzureContainerInfrastructureConfigurationElementValueResolverContext context)
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
}
