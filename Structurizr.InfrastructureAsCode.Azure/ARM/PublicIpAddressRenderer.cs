using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class PublicIpAddressRenderer : AzureResourceRenderer<PublicIpAddress>
    {
        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<PublicIpAddress> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var ip = Template(
                "Microsoft.Network/publicIPAddresses",
                elementWithInfrastructure.Infrastructure.Name,
                location,
                "2018-08-01");

            ip["sku"] = new JObject
            {
                ["name"] = "Basic"
            };
            ip["zones"] = new JArray();
            ip["properties"] = new JObject
            {
                ["publicIPAllocationMethod"] = "Dynamic",
                ["idleTimeoutInMinutes"] = 4,
            };

            template.Resources.Add(PostProcess(ip));
        }
    }
}