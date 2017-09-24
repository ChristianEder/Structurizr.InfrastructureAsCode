using System;
using System.Linq;
using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode.Azure.ARM;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using TinyIoC;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public abstract class AzureInfrastructureRenderer : IInfrastructureRenderer
    {
        private readonly IResourceGroupTargetingStrategy _resourceGroupTargetingStrategy;
        private readonly IResourceLocationTargetingStrategy _resourceLocationTargetingStrategy;
        private readonly IAzureInfrastructureEnvironment _environment;
        protected readonly TinyIoCContainer Ioc;

        protected AzureInfrastructureRenderer(
            IResourceGroupTargetingStrategy resourceGroupTargetingStrategy,
            IResourceLocationTargetingStrategy resourceLocationTargetingStrategy,
            IAzureInfrastructureEnvironment environment,
            TinyIoCContainer ioc)
        {
            _resourceGroupTargetingStrategy = resourceGroupTargetingStrategy;
            _resourceLocationTargetingStrategy = resourceLocationTargetingStrategy;
            _environment = environment;
            Ioc = ioc;
        }

        public async Task Render(SoftwareSystemWithInfrastructure softwareSystem)
        {
            BeforeRender();
            try
            {
                var azureInfrastructureElements = softwareSystem.ElementsWithInfrastructure().Distinct();

                foreach (var elementsInLocation in azureInfrastructureElements.GroupBy(e => _resourceLocationTargetingStrategy.TargetLocation(_environment, e)))
                {
                    foreach (var elementsInResourceGroup in elementsInLocation.GroupBy(e => _resourceGroupTargetingStrategy.TargetResourceGroup(_environment, e)))
                    {
                        await DeployInfrastructure(elementsInResourceGroup.Key, elementsInLocation.Key, elementsInResourceGroup.ToArray(), softwareSystem.System.Name);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task DeployInfrastructure(string resourceGroupName, string location, IHaveInfrastructure[] elementsWithInfrastructure, string deploymentName)
        {
            await BeforeDeployInfrastructure(resourceGroupName, location);

            var template = ToTemplate(resourceGroupName, location, elementsWithInfrastructure);

            if (template.Resources.Any())
            {
                await DeployInfrastructure(resourceGroupName, location, template);
            }

            await AfterDeployInfrastructure(resourceGroupName, location, elementsWithInfrastructure);
        }

        private AzureDeploymentTemplate ToTemplate(string resourceGroupName, string location, IHaveInfrastructure[] elementsWithInfrastructure)
        {

            var template = new AzureDeploymentTemplate(TemplateDeploymentVersion(resourceGroupName));

            foreach (var elementWithInfrastructure in elementsWithInfrastructure)
            {
                var renderer = AzureResourceRendererExtensions.GetRendererFor(Ioc, elementWithInfrastructure);
                renderer?.Render(template, elementWithInfrastructure, _environment, resourceGroupName, location);
            }

            return template;
        }

        protected abstract string TemplateDeploymentVersion(string resourceGroupName);


        protected abstract void BeforeRender();
        protected abstract Task BeforeDeployInfrastructure(string resourceGroupName, string location);
        protected abstract Task DeployInfrastructure(string resourceGroupName, string location, AzureDeploymentTemplate template);
        protected abstract Task AfterDeployInfrastructure(string resourceGroupName, string location, IHaveInfrastructure[] elementsWithInfrastructure);
        protected abstract void AfterRender();
    }
}