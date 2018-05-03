using System;
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
        protected override void Render(AzureDeploymentTemplate template,
            IHaveInfrastructure<KeyVault> elementWithInfrastructure, IAzureInfrastructureEnvironment environment,
            string resourceGroup, string location)
        {
            var keyVault = elementWithInfrastructure.Infrastructure;

            template.Resources.Add(new JObject
            {
                ["type"] = "Microsoft.KeyVault/vaults",
                ["name"] = keyVault.Name,
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
                                ["keys"] = new JArray("Get", "List", "Update", "Create", "Import", "Delete",
                                        "Backup", "Restore"),
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
            });


            foreach (var keyVaultSecret in keyVault.Secrets)
            {
                if (!keyVaultSecret.Value.IsResolved)
                {
                    throw new Exception($"The keyvault secret {keyVaultSecret.Name} is not resolved ");
                }

                template.Resources.Add(new JObject
                {
                    ["type"] = "Microsoft.KeyVault/vaults/secrets",
                    ["name"] = keyVault.Name + "/" + keyVaultSecret.Name,
                    ["apiVersion"] = "2015-06-01",
                    ["properties"] = new JObject
                    {
                        ["contentType"] = "text/plain",
                        ["value"] = keyVaultSecret.Value.Value.ToString(),
                        ["dependsOn"] = GetDependsOn(keyVaultSecret, keyVault)
                    }
                });
            }
        }

        protected JArray GetDependsOn(KeyVaultSecret keyVaultSecret, KeyVault keyVault)
        {
            if (keyVaultSecret.Value is IDependentConfigurationValue value)
            {               
                return new JArray(keyVault.ResourceIdReference, value.DependsOn.ResourceIdReference);
            }

            else return new JArray(keyVault.ResourceIdReference);
        }


        protected override IEnumerable<IConfigurationValue> GetConfigurationValues(
            IHaveInfrastructure<KeyVault> elementWithInfrastructure)
        {
            return base.GetConfigurationValues(elementWithInfrastructure)
                .Concat(elementWithInfrastructure.Infrastructure.Secrets.Values);
        }

        protected override Task Configure(IHaveInfrastructure<KeyVault> elementWithInfrastructure,
            AzureConfigurationValueResolverContext context)
        {
            foreach (var secret in elementWithInfrastructure.Infrastructure.Secrets)
            {
                object value;
                if (context.Values.TryGetValue(secret.Value, out value))
                {
                    // TODO: add secret to key vault using the context.Client
                }
            }

            return Task.FromResult(1);
        }
    }
}