using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Microsoft.Azure.Management.Resource.Fluent;
using Microsoft.Azure.Management.Resource.Fluent.Core.CollectionActions;
using Microsoft.Azure.Management.Resource.Fluent.Models;
using Newtonsoft.Json.Linq;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public static class ResourceManagementClientExtensions
    {
        public static async Task EnsureResourceGroupExists(
            this IAppServiceManager appServiceManager, string resourceGroupName, string resourceGroupLocation)
        {
            var exists = appServiceManager.ResourceManager.ResourceGroups.CheckExistence(resourceGroupName);
            if (exists)
            {
                Console.WriteLine($"Using existing resource group '{resourceGroupName}'");
            }
            else
            {
                Console.WriteLine($"Creating resource group '{resourceGroupName}' in location '{resourceGroupLocation}'");

                await appServiceManager.ResourceManager.ResourceGroups
                    .Define(resourceGroupName)
                    .WithRegion(resourceGroupLocation)
                    .CreateAsync();

                Console.WriteLine($"... Done");
            }
        }

        public static async Task Deploy(this IAppServiceManager appServiceManager, string resourceGroupName, string resourceGroupLocation, JObject template, string deploymentName)
        {
            if (template == null)
            {
                return;
            }

            Console.WriteLine($"Starting template deployment '{deploymentName}' in resource group '{resourceGroupName}'");
            
            appServiceManager.ResourceManager.Deployments
                .Define(deploymentName)
                .WithExistingResourceGroup(resourceGroupName)
                .WithTemplate(template.ToString())
                .WithParameters(new {})
                .WithMode(DeploymentMode.Incremental)
                .BeginCreate();

            var deployment = await appServiceManager.ResourceManager.Deployments.GetByGroupAsync(resourceGroupName, deploymentName);


            Console.WriteLine($"Deployment status: {deployment.ProvisioningState}");
            var lastProvisioningState = deployment.ProvisioningState;



            while (!IsCompleted(deployment.ProvisioningState))
            {
                if (lastProvisioningState != deployment.ProvisioningState)
                {
                    Console.Write($" {deployment.ProvisioningState} ");
                    lastProvisioningState = deployment.ProvisioningState;
                }

                Console.Write(".");
                await Task.Delay(500);

                deployment = await appServiceManager.ResourceManager.Deployments.GetByGroupAsync(resourceGroupName, deploymentName);
            }
            Console.WriteLine();
            Console.WriteLine($"Deployment status: {deployment.ProvisioningState}");
        }
        
        private static bool IsCompleted(string provisioningState)
        {
            return provisioningState == ProvisioningState.Succeeded.ToString() ||
                provisioningState == ProvisioningState.Failed.ToString() ||
                provisioningState == ProvisioningState.Canceled.ToString() ||
                provisioningState == ProvisioningState.Deleting.ToString();
            
        }
    }
}
