using System.Linq;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class KeyVaultRenderer : AzureResourceRenderer<KeyVault>
    {
        public override JObject Render(Container container, IInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var keyVault = (KeyVault)container.Infrastructure;

            return new JObject
            {
                ["type"] = "Microsoft.KeyVault/vaults",
                ["name"] = keyVault.Name(environment),
                ["apiVersion"] = "2015-06-01",
                ["location"] = location,
                ["properties"] = new JObject
                {
                    ["enabledForDeployment"] = false,
                    ["enabledForTemplateDeployment"] = false,
                    ["enabledForDiskEncryption"] = false,
                    ["accessPolicies"] = new JArray(
                        environment.AdministratorUserIds.Select(s => new JObject
                        {
                           ["tenantId"] = environment.Tenant,
                           ["objectId"] = s,
                           ["permissions"] = new JObject
                           {
                               ["keys"] = new JArray("Get", "List", "Update", "Create", "Import", "Delete", "Backup", "Restore"),
                               ["secrets"] = new JArray("All")
                           }
                        })
                        .Cast<object>().ToArray()
                        ),
                    ["tenantId"] = environment.Tenant,
                    ["sku"] = new JObject
                    {
                        ["name"] = "Standard",
                        ["family"] = "A"
                    }
                }
            };
        }
    }
}