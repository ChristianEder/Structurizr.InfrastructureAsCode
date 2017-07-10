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
                var azureInfrastructureElements = softwareSystem.ElementsWithInfrastructure().Distinct();

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

        private async Task DeployInfrastructure(IAzure azure, IGraphRbacManagementClient graph, string resourceGroupName, string location, IHaveInfrastructure[] elementsWithInfrastructure, string deploymentName)
        {
            var configContext = SetContextToConfigurationResolvers(azure, graph, resourceGroupName);

            await azure.EnsureResourceGroupExists(resourceGroupName, location);

            var deployments = azure.Deployments.List()
                .Where(d => d.ResourceGroupName == resourceGroupName)
                .Distinct()
                .Count();

            var template = ToTemplate(resourceGroupName, location, elementsWithInfrastructure, deployments);

            if (template.Resources.Any())
            {
                await azure.Deploy(resourceGroupName, location, template, $"{deploymentName}.{deployments}");
            }

            await Configure(elementsWithInfrastructure, configContext);
        }

        private async Task Configure(IHaveInfrastructure[] elementsWithInfrastructure, AzureConfigurationValueResolverContext configContext)
        {
            await ResolveConfigurationValuesToContext(elementsWithInfrastructure, configContext);
            foreach (var container in elementsWithInfrastructure)
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

        private async Task ResolveConfigurationValuesToContext(IEnumerable<IHaveInfrastructure> elementsWithInfrastructure,
            AzureConfigurationValueResolverContext configContext)
        {
            var valuesAndResolvers = elementsWithInfrastructure.SelectMany(c =>
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

        private AzureDeploymentTemplate ToTemplate(string resourceGroupName, string location, IHaveInfrastructure[] elementsWithInfrastructure, int deploymentsCount)
        {
            var template = new AzureDeploymentTemplate($"1.0.0.{deploymentsCount}");

            foreach (var elementWithInfrastructure in elementsWithInfrastructure)
            {
                var renderer = _ioc.GetRendererFor(elementWithInfrastructure);
                if (renderer != null)
                {
                    renderer.Render(template, elementWithInfrastructure, _environment, resourceGroupName, location);
                }
            }

            return template;
        }
        
        private AzureConfigurationValueResolverContext SetContextToConfigurationResolvers(IAzure azure, IGraphRbacManagementClient graph, string resourceGroupName)
        {

            var configContext = new AzureConfigurationValueResolverContext(azure, graph, resourceGroupName);
            _ioc.Register(configContext);

            return configContext;
        }
    }
}
