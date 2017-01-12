using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.ARM.Configuration;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class KeyVaultRenderer : AzureResourceRenderer<KeyVault>
    {
        protected override IEnumerable<JObject> Render(Container<KeyVault> container, IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            yield return new JObject
            {
                ["type"] = "Microsoft.KeyVault/vaults",
                ["name"] = container.Infrastructure.Name,
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

        protected override IEnumerable<ContainerInfrastructureConfigurationElementValue> GetConfigurationValues(Container<KeyVault> container)
        {
            return base.GetConfigurationValues(container).Concat(container.Infrastructure.Secrets.Values);
        }

        protected override async Task Configure(Container<KeyVault> container, AzureContainerInfrastructureConfigurationElementValueResolverContext context)
        {
            foreach (var secret in container.Infrastructure.Secrets)
            {
                object value;
                if (context.Values.TryGetValue(secret.Value, out value))
                {
                    // TODO: add secret to key vault using the context.Client
                }
            }
        }
    }
}