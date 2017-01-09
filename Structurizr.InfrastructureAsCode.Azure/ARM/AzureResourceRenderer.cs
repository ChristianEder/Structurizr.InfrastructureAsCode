using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public abstract class AzureResourceRenderer
    {
        public abstract bool CanRender(Container container);
        public abstract JObject Render(Container container, IInfrastructureEnvironment environment, string resourceGroup, string location);
    }

    public abstract class AzureResourceRenderer<TInfrastructure> : AzureResourceRenderer
        where TInfrastructure : ContainerInfrastructure
    {
        public override bool CanRender(Container container)
        {
            return container.Infrastructure is TInfrastructure;
        }
    }
}
