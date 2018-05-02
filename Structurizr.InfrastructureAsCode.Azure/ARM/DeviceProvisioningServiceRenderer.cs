using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class DeviceProvisioningServiceRenderer : AzureResourceRenderer<DeviceProvisioningService>
    {
        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<DeviceProvisioningService> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var dps = elementWithInfrastructure.Infrastructure;

            template.Resources.Add(new JObject
            {
                ["type"] = "Microsoft.Devices/provisioningServices",
                ["name"] = dps.Name,
                ["apiVersion"] = dps.ApiVersion,
                ["location"] = location 
            });
        }
    }
}
