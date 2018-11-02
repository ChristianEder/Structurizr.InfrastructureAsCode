using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class StorageAccountRenderer : AzureResourceRenderer<StorageAccount>
    {
        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<StorageAccount> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var storageAccount = Template(
                "Microsoft.Storage/storageAccounts",
                elementWithInfrastructure.Infrastructure.Name,
                location,
                "2017-10-01"
            );

            storageAccount["sku"] = new JObject
            {
                ["name"] = "Standard_LRS"
            };

            storageAccount["kind"] = elementWithInfrastructure.Infrastructure.Kind.ToString();

            var properties = new JObject
            {
                ["supportsHttpsTrafficOnly"] = true,
                ["accessTier"] = "Hot",
                ["encryption"] = new JObject
                {
                    ["keySource"] = "Microsoft.Storage",
                    ["services"] = new JObject
                    {
                        ["blob"] = new JObject { ["enabled"] = true },
                        ["file"] = new JObject { ["enabled"] = true }
                    }
                }
            };
          
            storageAccount["properties"] = properties;

            template.Resources.Add(PostProcess(storageAccount));
        }
    }
}