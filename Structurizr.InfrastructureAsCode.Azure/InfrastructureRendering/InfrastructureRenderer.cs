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
using Structurizr.InfrastructureAsCode.Azure.ARM.Configuration;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;
using TinyIoC;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class InfrastructureRenderer : IInfrastructureRenderer<IAzureInfrastructureEnvironment>
    {
        private readonly IResourceGroupTargetingStrategy _resourceGroupTargetingStrategy;
        private readonly IResourceLocationTargetingStrategy _resourceLocationTargetingStrategy;
        private readonly IAzureSubscriptionCredentials _subscriptionCredentials;
        private readonly TinyIoCContainer _ioc;

        public InfrastructureRenderer(
            IResourceGroupTargetingStrategy resourceGroupTargetingStrategy,
            IResourceLocationTargetingStrategy resourceLocationTargetingStrategy,
            IAzureSubscriptionCredentials subscriptionCredentials,
            TinyIoCContainer ioc)
        {
            _resourceGroupTargetingStrategy = resourceGroupTargetingStrategy;
            _resourceLocationTargetingStrategy = resourceLocationTargetingStrategy;
            _subscriptionCredentials = subscriptionCredentials;
            _ioc = ioc;
        }

        public async Task Render(Structurizr.Model model, IAzureInfrastructureEnvironment environment)
        {
            var client = await LoginClient();

            var configContext = SetContextToConfigurationResolvers(client);

            foreach (var softwareSystem in model.SoftwareSystems)
            {
                var azureInfrastructureElements = softwareSystem.Containers
                     .OfType<Container>()
                     .Distinct();

                foreach (var elementsInLocation in azureInfrastructureElements.GroupBy(e => _resourceLocationTargetingStrategy.TargetLocation(environment, e)))
                {
                    foreach (var elementsInResourceGroup in elementsInLocation.GroupBy(e => _resourceGroupTargetingStrategy.TargetResourceGroup(environment, e)))
                    {
                        await DeployInfrastructure(client, environment, elementsInResourceGroup.Key, elementsInLocation.Key, elementsInResourceGroup.ToArray(), softwareSystem.Name, configContext);
                    }
                }
            }
        }
        
        private async Task DeployInfrastructure(ResourceManagementClient client, IAzureInfrastructureEnvironment environment, string resourceGroupName, string location, Container[] containers, string deploymentName, AzureConfigurationValueResolverContext configContext)
        {
            await client.EnsureResourceGroupExists(resourceGroupName, location);

            var deployments = await client.Deployments.ListAsync(resourceGroupName, new DeploymentListParameters());
            var template = ToTemplate(resourceGroupName, environment, location, containers, deployments.Deployments.Count);
            await client.Deploy(resourceGroupName, location, template, $"{deploymentName}.{deployments.Deployments.Count}");

            //await Configure(environment, containers, configContext);
        }

        private async Task Configure(IAzureInfrastructureEnvironment environment, Container[] containers, AzureConfigurationValueResolverContext configContext)
        {
            await ResolveConfigurationValuesToContext(containers, configContext);
            foreach (var container in containers)
            {
                var renderer = _ioc.GetRendererFor(container);
                if (renderer != null)
                {
                    await renderer.Configure(container, configContext);
                }
            }
        }

        private async Task ResolveConfigurationValuesToContext(IEnumerable<Container> containers,
            AzureConfigurationValueResolverContext configContext)
        {
            var valuesAndResolvers = containers.SelectMany(c =>
            {
                var renderer = _ioc.GetRendererFor(c);
                return renderer != null
                    ? renderer.GetConfigurationValues(c)
                    : Enumerable.Empty<ConfigurationValue>();
            }).ToDictionary(v => v, v => _ioc.GetResolverFor(v));

            var value = FindFirstValueToBeResolved(valuesAndResolvers);
            while (value != null)
            {
                var resolver = valuesAndResolvers[value];
                valuesAndResolvers.Remove(value);

                var resolvedValue = await resolver.Resolve(value);
                configContext.Values.Add(value, resolvedValue);

                value = FindFirstValueToBeResolved(valuesAndResolvers);
            }
        }

        private ConfigurationValue FindFirstValueToBeResolved(Dictionary<ConfigurationValue, IConfigurationValueResolver> valuesAndResolvers)
        {
            if (!valuesAndResolvers.Any())
            {
                return null;
            }

            foreach (var valueAndResolver in valuesAndResolvers)
            {

                if (valueAndResolver.Value.CanResolve(valueAndResolver.Key))
                {
                    return valueAndResolver.Key;
                }
            }
            return null;
        }

        private JObject ToTemplate(string resourceGroupName, IAzureInfrastructureEnvironment environment, string location, IEnumerable<Container> containers, int deploymentsCount)
        {
            var template = new JObject
            {
                ["$schema"] = "http://schema.management.azure.com/schemas/2014-04-01-preview/deploymentTemplate.json#",
                ["contentVersion"] = $"1.0.0.{deploymentsCount}",
                ["parameters"] = new JObject(),
                ["resources"] = new JArray(
                    containers.SelectMany(e => ToResource(e, environment, resourceGroupName, location))
                    .Where(r => r != null)
                    .Cast<object>()
                    .ToArray())
            };

            return template;
        }

        private IEnumerable<JObject> ToResource(Container container, IAzureInfrastructureEnvironment environment, string resourceGroupName, string location)
        {
            var renderer = _ioc.GetRendererFor(container);
            return renderer?.Render(container, environment, resourceGroupName, location);
        }

        private async Task<ResourceManagementClient> LoginClient()
        {
            var credentials = await GetAccessTokenCredentials();
            return new ResourceManagementClient(credentials);
        }

        private async Task<SubscriptionCloudCredentials> GetAccessTokenCredentials()
        {
            var clientCredential = new ClientCredential(_subscriptionCredentials.ClientId, _subscriptionCredentials.ClientSecret);
            var context = new AuthenticationContext($"https://login.microsoftonline.com/{_subscriptionCredentials.TenantId}");
            var result = await context.AcquireTokenAsync("https://management.azure.com/", clientCredential);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain authorization token");
            }

            return new TokenCloudCredentials(_subscriptionCredentials.SubscriptionId, result.AccessToken);
        }

        private AzureConfigurationValueResolverContext SetContextToConfigurationResolvers(ResourceManagementClient client)
        {
            var configContext = new AzureConfigurationValueResolverContext(client);
            _ioc.Register(configContext);
            return configContext;
        }
    }
}
