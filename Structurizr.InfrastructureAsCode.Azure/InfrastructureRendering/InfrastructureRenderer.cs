using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.ARM;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class InfrastructureRenderer : IInfrastructureRenderer
    {
        private readonly IResourceGroupTargetingStrategy _resourceGroupTargetingStrategy;
        private readonly IResourceLocationTargetingStrategy _resourceLocationTargetingStrategy;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tenantId;
        private readonly string _subscriptionId;

        public InfrastructureRenderer(
            IResourceGroupTargetingStrategy resourceGroupTargetingStrategy,
            IResourceLocationTargetingStrategy resourceLocationTargetingStrategy,
            string clientId,
            string clientSecret,
            string tenantId,
            string subscriptionId)
        {
            _resourceGroupTargetingStrategy = resourceGroupTargetingStrategy;
            _resourceLocationTargetingStrategy = resourceLocationTargetingStrategy;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _tenantId = tenantId;
            _subscriptionId = subscriptionId;

            Renderers = GetType().Assembly.GetTypes()
                .Where(t => typeof(AzureResourceRenderer).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<AzureResourceRenderer>()
                .ToList();
        }

        public async Task Render(Structurizr.Model model, IInfrastructureEnvironment environment)
        {
            var client = await LoginClient();


            foreach (var softwareSystem in model.SoftwareSystems)
            {
                var azureInfrastructureElements = softwareSystem.Containers
                     .OfType<Container>()
                     .Distinct();

                foreach (var elementsInLocation in azureInfrastructureElements.GroupBy(e => _resourceLocationTargetingStrategy.TargetLocation(environment, e)))
                {
                    foreach (var elementsInResourceGroup in elementsInLocation.GroupBy(e => _resourceGroupTargetingStrategy.TargetResourceGroup(environment, e)))
                    {
                        await DeployInfrastructure(client, environment, elementsInResourceGroup.Key, elementsInLocation.Key, elementsInResourceGroup, softwareSystem.Name);
                    }
                }
            }
        }

        public List<AzureResourceRenderer> Renderers { get; }

        private async Task DeployInfrastructure(ResourceManagementClient client, IInfrastructureEnvironment environment, string resourceGroupName, string location, IEnumerable<Container> containers, string deploymentName)
        {
            var deployments = await client.Deployments.ListAsync(resourceGroupName, new DeploymentListParameters());
            await client.EnsureResourceGroupExists(resourceGroupName, location);
            var template = ToTemplate(resourceGroupName, environment, location, containers, deployments.Deployments.Count);
            await client.Deploy(resourceGroupName, location, template, $"{deploymentName}.{deployments.Deployments.Count}");
        }

        private JObject ToTemplate(string resourceGroupName, IInfrastructureEnvironment environment, string location, IEnumerable<Container> containers, int deploymentsCount)
        {
            var template = new JObject
            {
                ["$schema"] = "http://schema.management.azure.com/schemas/2014-04-01-preview/deploymentTemplate.json#",
                ["contentVersion"] = $"1.0.0.{deploymentsCount}",
                ["parameters"] = new JObject(),
                ["resources"] = new JArray(
                    containers.Select(e => ToResource(e, environment, resourceGroupName, location))
                    .Where(r => r != null)
                    .Cast<object>()
                    .ToArray())
            };

            return template;
        }

        private JObject ToResource(Container container, IInfrastructureEnvironment environment, string resourceGroupName, string location)
        {
            var renderer = Renderers.FirstOrDefault(r => r.CanRender(container));
            return renderer?.Render(container, environment, resourceGroupName, location);
        }

        private async Task<ResourceManagementClient> LoginClient()
        {
            var credentials = await GetAccessTokenCredentials();
            return new ResourceManagementClient(credentials);
        }

        private async Task<SubscriptionCloudCredentials> GetAccessTokenCredentials()
        {
            var clientCredential = new ClientCredential(_clientId, _clientSecret);
            var context = new AuthenticationContext($"https://login.microsoftonline.com/{_tenantId}");
            var result = await context.AcquireTokenAsync("https://management.azure.com/", clientCredential);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain authorization token");
            }

            return new TokenCloudCredentials(_subscriptionId, result.AccessToken);
        }
    }
}
