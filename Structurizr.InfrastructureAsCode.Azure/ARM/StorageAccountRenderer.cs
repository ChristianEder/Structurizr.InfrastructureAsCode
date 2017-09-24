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
                "2017-06-01"
            );

            const string accountType = "Standard_LRS";

            storageAccount["sku"] = new JObject
            {
                ["name"] = accountType
            };

            storageAccount["kind"] = elementWithInfrastructure.Infrastructure.Kind.ToString();

            var isBlobStorage = elementWithInfrastructure.Infrastructure.Kind == StorageAccountKind.BlobStorage;

            var properties = new JObject
            {
                ["supportsHttpsTrafficOnly"] = false,
                ["encryption"] = new JObject
                {
                    ["keySource"] = "Microsoft.Storage",
                    ["services"] = new JObject
                    {
                        ["blob"] = new JObject { ["enabled"] = true },
                        ["file"] = new JObject { ["enabled"] = !isBlobStorage }
                    }
                }
            };
            if (isBlobStorage)
            {
                properties["accessTier"] = "Hot";
            }
            storageAccount["properties"] = properties;

            template.Resources.Add(storageAccount);
        }
    }
}