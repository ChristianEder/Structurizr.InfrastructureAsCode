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
            var deploymentState = deploymentResult.Deployment;

            Console.WriteLine($"Deployment status: {deploymentState.Properties.ProvisioningState}");
            var lastProvisioningState = deploymentState.Properties.ProvisioningState;

            while (!IsCompleted(deploymentState))
            {
                if (lastProvisioningState != deploymentState.Properties.ProvisioningState)
                {
                    Console.Write($" {deploymentState.Properties.ProvisioningState} ");
                    lastProvisioningState = deploymentState.Properties.ProvisioningState;
                }

                Console.Write(".");
                await Task.Delay(500);
                var current = await client.Deployments.GetAsync(resourceGroupName, deploymentName);
                deploymentState = current.Deployment;
            }
            Console.WriteLine();
            Console.WriteLine($"Deployment status: {deploymentState.Properties.ProvisioningState}");
        }

        private static bool IsCompleted(DeploymentExtended deployment)
        {
            return deployment.Properties.ProvisioningState == ProvisioningState.Succeeded ||
                deployment.Properties.ProvisioningState == ProvisioningState.Failed ||
                deployment.Properties.ProvisioningState == ProvisioningState.Canceled ||
                deployment.Properties.ProvisioningState == ProvisioningState.Deleted;
            
        }
    }
}
