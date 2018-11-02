using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class EventHubNamespaceRenderer : AzureResourceRenderer<EventHubNamespace>
    {
        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<EventHubNamespace> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var eventHubNamespace = elementWithInfrastructure.Infrastructure;

            template.Resources.Add(PostProcess(new JObject
            {
                ["apiVersion"] = eventHubNamespace.ApiVersion,
                ["type"] = "Microsoft.EventHub/namespaces",
                ["name"] = eventHubNamespace.Name,
                ["location"] = location,
                ["sku"] = new JObject
                {
                    ["name"] = "Basic",
                    ["tier"] = "Basic",
                    ["capacity"] = 1
                },
                ["properties"] = new JObject(),
                ["resources"] = Resources(eventHubNamespace, location)
            }));
        }

        private JArray Resources(EventHubNamespace eventHubNamespace, string location)
        {
            var resources = new JArray();
            foreach (var hub in eventHubNamespace.Hubs)
            {
                resources.Add(Hub(eventHubNamespace, hub, location));
            }

            return resources;
        }

        private JObject Hub(EventHubNamespace eventHubNamespace, EventHub hub, string location)
        {
            return new JObject
            {
                ["apiVersion"] = eventHubNamespace.ApiVersion,
                ["type"] = "eventhubs",
                ["name"] = hub.Name,
                ["location"] = location,
                ["properties"] = new JObject
                {
                    ["messageRetentionInDays"] = 1
                },
                ["dependsOn"] = new JArray(eventHubNamespace.ResourceIdReference)
            };
        }
    }
}