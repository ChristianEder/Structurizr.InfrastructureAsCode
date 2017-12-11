using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class IoTHubRenderer: AzureResourceRenderer<IoTHub>
    {
        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<IoTHub> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var hub = elementWithInfrastructure.Infrastructure;

            template.Resources.Add(new JObject
            {
                ["apiVersion"] = hub.ApiVersion,
                ["type"] = "Microsoft.Devices/Iothubs",
                ["name"] = hub.Name,
                ["location"] = location,
                ["sku"] = new JObject
                {
                    ["name"] = "Free",
                    ["tier"] = "F1",
                    ["capacity"] = 1
                },
                ["properties"] = new JObject
                {
                    ["location"] = location
                }
            });

            foreach (var consumerGroup in hub.ConsumerGroups)
            {
                template.Resources.Add(new JObject
                {
                    ["apiVersion"] = hub.ApiVersion,
                    ["type"] = "Microsoft.Devices/Iothubs/eventhubEndpoints/ConsumerGroups",
                    ["name"] = $"{hub.Name}/events/{consumerGroup}",
                    ["dependsOn"] = new JArray
                    {
                        hub.ResourceIdReference
                    }
                });
            }
        }
    }
}