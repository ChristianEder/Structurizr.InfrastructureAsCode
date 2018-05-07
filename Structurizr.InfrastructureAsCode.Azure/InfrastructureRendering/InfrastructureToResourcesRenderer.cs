using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Structurizr.InfrastructureAsCode.Azure.ARM;
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
            return Task.CompletedTask;
        }

        private int DeploymentCount(string resourceGroupName)
        {
            return _azure.Deployments.List()
                .Where(d => d.ResourceGroupName == resourceGroupName)
                .Distinct()
                .Count();
        }
    }
}
