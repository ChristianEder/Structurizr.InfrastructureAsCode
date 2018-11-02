using System.Linq;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class DeviceProvisioningServiceRenderer : AzureResourceRenderer<DeviceProvisioningService>
    {
        protected override void Render(AzureDeploymentTemplate template,
            IHaveInfrastructure<DeviceProvisioningService> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var dps = elementWithInfrastructure.Infrastructure;         

            template.Resources.Add(PostProcess(new JObject
            {
                ["type"] = "Microsoft.Devices/provisioningServices",
                ["name"] = dps.Name,
                ["apiVersion"] = dps.ApiVersion,
                ["location"] = location,
                ["properties"] = new JObject
                {
                    ["iotHubs"] = new JArray(dps.IotHubs.Select(iothub =>
                        new JObject
                        {
                            ["name"] = iothub.Url,
                            ["connectionString"] = iothub.OwnerConnectionString.Value.ToString(),
                            ["location"] = location
                        }))
                },
                ["dependsOn"] = new JArray(dps.IotHubs.Select(iothub => iothub.ResourceIdReference).ToArray())
            }));
        }
    }
}