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

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class InfrastructureRenderer : IInfrastructureRenderer<IAzureInfrastructureEnvironment>
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

            Renderers = Find<AzureResourceRenderer>();
            ConfigurationValueResolvers = Find<ContainerInfrastructureConfigurationElementValueResolver>();
            ConfigurationValueResolvers.Add(new FixedContainerInfrastructureConfigurationElementValueResolver<string>());
            ConfigurationValueResolvers.Add(new FixedContainerInfrastructureConfigurationElementValueResolver<int>());
        }

        private List<T> Find<T>()
        {
            return GetType().Assembly.GetTypes()
                .Where(t => typeof(T).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Select(Activator.CreateInstance)
                .Cast<T>()
                .ToList();
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
        
        public List<AzureResourceRenderer> Renderers { get; }
        public List<ContainerInfrastructureConfigurationElementValueResolver> ConfigurationValueResolvers { get; }

        private async Task DeployInfrastructure(ResourceManagementClient client, IAzureInfrastructureEnvironment environment, string resourceGroupName, string location, Container[] containers, string deploymentName, AzureContainerInfrastructureConfigurationElementValueResolverContext configContext)
        {
            var deployments = await client.Deployments.ListAsync(resourceGroupName, new DeploymentListParameters());
            await client.EnsureResourceGroupExists(resourceGroupName, location);
            var template = ToTemplate(resourceGroupName, environment, location, containers, deployments.Deployments.Count);
            await client.Deploy(resourceGroupName, location, template, $"{deploymentName}.{deployments.Deployments.Count}");

            //await Configure(environment, containers, configContext);
        }

        private async Task Configure(IAzureInfrastructureEnvironment environment, Container[] containers, AzureContainerInfrastructureConfigurationElementValueResolverContext configContext)
        {
            await ResolveConfigurationValuesToContext(containers, configContext);
            foreach (var container in containers)
            {
                var renderer = Renderers.FirstOrDefault(r => r.CanConfigure(container));
                if (renderer != null)
                {
                    await renderer.Configure(container, configContext);
                }
            }
        }

        private async Task ResolveConfigurationValuesToContext(IEnumerable<Container> containers,
            AzureContainerInfrastructureConfigurationElementValueResolverContext configContext)
        {
            var values = containers.SelectMany(c =>
            {
                var renderer = Renderers.FirstOrDefault(r => r.CanConfigure(c));
                return renderer != null
                    ? renderer.GetConfigurationValues(c)
                    : Enumerable.Empty<ContainerInfrastructureConfigurationElementValue>();
            }).ToList();

            var valueAndResolver = FindFirstValueToBeResolved(values);
            while (valueAndResolver != null)
            {
                values.Remove(valueAndResolver.Item1);

                var value = await valueAndResolver.Item2.Resolve(valueAndResolver.Item1);
                configContext.Values.Add(valueAndResolver.Item1, value);

                valueAndResolver = FindFirstValueToBeResolved(values);
            }
        }

        private Tuple<ContainerInfrastructureConfigurationElementValue, ContainerInfrastructureConfigurationElementValueResolver> FindFirstValueToBeResolved(List<ContainerInfrastructureConfigurationElementValue> values)
        {
            if (!values.Any())
            {
                return null;
            }

            foreach (var value in values)
            {
                var resolver = ConfigurationValueResolvers.FirstOrDefault(r => r.CanResolve(value));
                if (resolver != null)
                {
                    return new Tuple<ContainerInfrastructureConfigurationElementValue, ContainerInfrastructureConfigurationElementValueResolver>(value, resolver);
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

        private AzureContainerInfrastructureConfigurationElementValueResolverContext SetContextToConfigurationResolvers(ResourceManagementClient client)
        {
            var configContext = new AzureContainerInfrastructureConfigurationElementValueResolverContext(client);
            foreach (var resolver in ConfigurationValueResolvers
                .OfType<IAzureContainerInfrastructureConfigurationElementValueResolver>())
            {
                resolver.SetContext(configContext);
            }
            return configContext;
        }
    }
}
