using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.ARM;
using Structurizr.InfrastructureAsCode.Azure.ARM.Configuration;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;
using TinyIoC;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class InfrastructureToResourcesRenderer : AzureInfrastructureRenderer
    {
        private readonly IAzureConnector _azureConnector;
        private IAzure _azure;
        private IGraphRbacManagementClient _graph;

        public InfrastructureToResourcesRenderer(
            IResourceGroupTargetingStrategy resourceGroupTargetingStrategy,
            IResourceLocationTargetingStrategy resourceLocationTargetingStrategy,
            IAzureConnector azureConnector,
            IAzureInfrastructureEnvironment environment,
            TinyIoCContainer ioc) : base(resourceGroupTargetingStrategy, resourceLocationTargetingStrategy, environment, ioc)
        {
            _azureConnector = azureConnector;
        }

        protected override void BeforeRender()
        {
            _azure = _azureConnector.Azure();
            _graph = _azureConnector.Graph();
        }

        protected override void AfterRender()
        {
            _graph.Dispose();
            _azure = null;
            _graph = null;
        }

        protected override async Task BeforeDeployInfrastructure(string resourceGroupName, string location)
        {
            SetContextToConfigurationResolvers(_azure, _graph, resourceGroupName);
            await _azure.EnsureResourceGroupExists(resourceGroupName, location);
        }

        protected override string TemplateDeploymentVersion(string resourceGroupName)
        {
            return $"1.0.0.{DeploymentCount(resourceGroupName)}";
        }

        protected override Task DeployInfrastructure(string resourceGroupName, string location, AzureDeploymentTemplate template)
        {
            return _azure.Deploy(resourceGroupName, location, template, $"{resourceGroupName}.{DeploymentCount(resourceGroupName)}");
        }

        protected override Task AfterDeployInfrastructure(string resourceGroupName, string location, List<IHaveInfrastructure> elementsWithInfrastructure)
        {
            return Configure(elementsWithInfrastructure);
        }

        private int DeploymentCount(string resourceGroupName)
        {
            return _azure.Deployments.List()
                .Where(d => d.ResourceGroupName == resourceGroupName)
                .Distinct()
                .Count();
        }

        private async Task Configure(List<IHaveInfrastructure> elementsWithInfrastructure)
        {
            var configContext = Ioc.Resolve<AzureConfigurationValueResolverContext>();

            await ResolveConfigurationValuesToContext(elementsWithInfrastructure, configContext);
            foreach (var container in elementsWithInfrastructure)
            {
                var renderer = Ioc.GetRendererFor(container);
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
                    var renderer = Ioc.GetRendererFor(c);
                    return renderer != null
                        ? renderer.GetConfigurationValues(c)
                        : Enumerable.Empty<IConfigurationValue>();
                })
                .Where(v => !v.IsResolved)
                .Distinct()
                .ToDictionary(v => v, v => Ioc.GetResolverFor(v));

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

        private IConfigurationValue FindFirstValueToBeResolved(Dictionary<IConfigurationValue, IConfigurationValueResolver> valuesAndResolvers)
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

        
        private void SetContextToConfigurationResolvers(IAzure azure, IGraphRbacManagementClient graph, string resourceGroupName)
        {
            var configContext = new AzureConfigurationValueResolverContext(azure, graph, resourceGroupName);
            Ioc.Register(configContext);
        }
    }
}
