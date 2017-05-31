using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.ARM;
using Structurizr.InfrastructureAsCode.Azure.ARM.Configuration;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;
using TinyIoC;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class InfrastructureRenderer : IInfrastructureRenderer
    {
        private readonly IResourceGroupTargetingStrategy _resourceGroupTargetingStrategy;
        private readonly IResourceLocationTargetingStrategy _resourceLocationTargetingStrategy;
        private readonly IAzureConnector _azureConnector;
        private readonly IAzureInfrastructureEnvironment _environment;
        private readonly TinyIoCContainer _ioc;

        public InfrastructureRenderer(
            IResourceGroupTargetingStrategy resourceGroupTargetingStrategy,
            IResourceLocationTargetingStrategy resourceLocationTargetingStrategy,
            IAzureConnector azureConnector,
            IAzureInfrastructureEnvironment environment,
            TinyIoCContainer ioc)
        {
            _resourceGroupTargetingStrategy = resourceGroupTargetingStrategy;
            _resourceLocationTargetingStrategy = resourceLocationTargetingStrategy;
            _azureConnector = azureConnector;
            _environment = environment;
            _ioc = ioc;
        }

        public async Task Render(SoftwareSystemWithInfrastructure softwareSystem)
        {
            var azure = _azureConnector.Azure();
            var graph = _azureConnector.Graph();

            try
            {
                var azureInfrastructureElements = softwareSystem.Containers().Distinct();

                foreach (var elementsInLocation in azureInfrastructureElements.GroupBy(e => _resourceLocationTargetingStrategy.TargetLocation(_environment, e)))
                {
                    foreach (var elementsInResourceGroup in elementsInLocation.GroupBy(e => _resourceGroupTargetingStrategy.TargetResourceGroup(_environment, e)))
                    {
                        await DeployInfrastructure(azure, graph, elementsInResourceGroup.Key, elementsInLocation.Key, elementsInResourceGroup.ToArray(), softwareSystem.System.Name);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task DeployInfrastructure(IAzure azure, IGraphRbacManagementClient graph, string resourceGroupName, string location, ContainerWithInfrastructure[] containers, string deploymentName)
        {
            var configContext = SetContextToConfigurationResolvers(azure, graph, resourceGroupName);

            await azure.EnsureResourceGroupExists(resourceGroupName, location);

            var deployments = azure.Deployments.List()
                .Where(d => d.ResourceGroupName == resourceGroupName)
                .Distinct()
                .Count();

            var template = ToTemplate(resourceGroupName, location, containers, deployments);

            if (template != null)
            {
                await azure.Deploy(resourceGroupName, location, template, $"{deploymentName}.{deployments}");
            }

            await Configure(containers, configContext);
        }

        private async Task Configure(ContainerWithInfrastructure[] containers, AzureConfigurationValueResolverContext configContext)
        {
            await ResolveConfigurationValuesToContext(containers, configContext);
            foreach (var container in containers)
            {
                var renderer = _ioc.GetRendererFor(container);
                if (renderer != null)
                {
                    Console.Write($"Configuring {container.Infrastructure.Name} ...");

                    await renderer.Configure(container, configContext);

                    Console.WriteLine(" done");
                }
            }
        }

        private async Task ResolveConfigurationValuesToContext(IEnumerable<ContainerWithInfrastructure> containers,
            AzureConfigurationValueResolverContext configContext)
        {
            var valuesAndResolvers = containers.SelectMany(c =>
                {
                    var renderer = _ioc.GetRendererFor(c);
                    return renderer != null
                        ? renderer.GetConfigurationValues(c)
                        : Enumerable.Empty<ConfigurationValue>();
                })
                .ToDictionary(v => v, v => _ioc.GetResolverFor(v));

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
                if (valueAndResolver.Value != null && 
                    valueAndResolver.Value.CanResolve(valueAndResolver.Key))
                {
                    return valueAndResolver.Key;
                }
            }
            return null;
        }

        private JObject ToTemplate(string resourceGroupName, string location, IEnumerable<ContainerWithInfrastructure> containers, int deploymentsCount)
        {
            var resources = containers.Select(e => ToResource(e, resourceGroupName, location))
                .Where(r => r != null)
                .SelectMany(r => r)
                .Where(r => r != null)
                .Cast<object>()
                .ToArray();

            if (!resources.Any())
            {
                return null;
            }

            var template = new JObject
            {
                ["$schema"] = "http://schema.management.azure.com/schemas/2014-04-01-preview/deploymentTemplate.json#",
                ["contentVersion"] = $"1.0.0.{deploymentsCount}",
                ["parameters"] = new JObject(),
                ["resources"] = new JArray(resources)
            };

            return template;
        }

        private IEnumerable<JObject> ToResource(ContainerWithInfrastructure container, string resourceGroupName, string location)
        {
            var renderer = _ioc.GetRendererFor(container);
            return renderer?.Render(container, _environment, resourceGroupName, location);
        }




        private AzureConfigurationValueResolverContext SetContextToConfigurationResolvers(IAzure azure, IGraphRbacManagementClient graph, string resourceGroupName)
        {

            var configContext = new AzureConfigurationValueResolverContext(azure, graph, resourceGroupName);
            _ioc.Register(configContext);

            return configContext;



        }
    }
}
