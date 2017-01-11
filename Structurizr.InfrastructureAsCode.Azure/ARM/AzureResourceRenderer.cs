using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public abstract class AzureResourceRenderer
    {
        public abstract bool CanRender(Container container);
        public abstract JObject Render(Container container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location);
    }

    public abstract class AzureResourceRenderer<TInfrastructure> : AzureResourceRenderer
        where TInfrastructure : ContainerInfrastructure
    {
        public override bool CanRender(Container container)
        {
            return container is Container<TInfrastructure>;
        }

        public override JObject Render(Container container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            return Render((Container<TInfrastructure>) container, environment, resourceGroup, location);
        }

        protected abstract JObject Render(Container<TInfrastructure> container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location);
    }
}
