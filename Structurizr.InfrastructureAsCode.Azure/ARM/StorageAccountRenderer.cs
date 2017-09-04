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
                "2015-05-01-preview"
            );

            storageAccount["properties"] = new JObject
            {
                ["accountType"] = "Standard_LRS"
            };

            template.Resources.Add(storageAccount);
        }
    }
}