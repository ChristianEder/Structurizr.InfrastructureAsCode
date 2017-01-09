using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Newtonsoft.Json.Linq;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public static class ResourceManagementClientExtensions
    {
        public static async Task EnsureResourceGroupExists(
            this ResourceManagementClient client, string resourceGroupName, string resourceGroupLocation)
        {
            var exists = await client.ResourceGroups.CheckExistenceAsync(resourceGroupName);
            if (exists.Exists)
            {
                Console.WriteLine($"Using existing resource group '{resourceGroupName}'");
            }
            else
            {
                Console.WriteLine($"Creating resource group '{resourceGroupName}' in location '{resourceGroupLocation}'");
                var resourceGroup = new ResourceGroup {Location = resourceGroupLocation};
                await client.ResourceGroups.CreateOrUpdateAsync(resourceGroupName, resourceGroup);
                Console.WriteLine($"... Done");
            }
        }

        public static async Task Deploy(this ResourceManagementClient client, string resourceGroupName, string resourceGroupLocation, JObject template, string deploymentName)
        {
            if (template == null)
            {
                return;
            }

            Console.WriteLine($"Starting template deployment '{deploymentName}' in resource group '{resourceGroupName}'");
            var deployment = new Deployment
            {
                Properties = new DeploymentProperties
                {
                    Mode = DeploymentMode.Incremental,
                    Template = template.ToString(),
                }
            };

            var deploymentResult = await client.Deployments.CreateOrUpdateAsync(resourceGroupName, deploymentName, deployment);
            Console.WriteLine($"Deployment status: {deploymentResult.Deployment.Properties.ProvisioningState}");
        }
    }
}
