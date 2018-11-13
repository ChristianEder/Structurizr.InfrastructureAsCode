using System.Linq;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class VirtualNetworkRenderer : AzureResourceRenderer<VirtualNetwork>
    {
        protected override void Render(AzureDeploymentTemplate template,
            IHaveInfrastructure<VirtualNetwork> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var network = Template(
                "Microsoft.Network/virtualNetworks",
                elementWithInfrastructure.Infrastructure.Name,
                location,
                "2018-08-01");

            network["properties"] = new JObject
            {
                ["addressSpace"] = new JObject
                {
                    ["addressPrefixes"] = new JArray(elementWithInfrastructure.Infrastructure.Prefix)
                },
                ["subnets"] = new JArray(elementWithInfrastructure.Infrastructure.Subnets.Select(subnet => new JObject
                {
                    ["name"] = subnet.Name,
                    ["properties"] = new JObject
                    {
                        ["addressPrefix"] = subnet.Prefix
                    }
                }))
            };

            template.Resources.Add(PostProcess(network));
        }
    }
}