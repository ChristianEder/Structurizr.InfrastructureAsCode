using System;
using System.IO;
using System.Threading.Tasks;
using TinyIoC;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public interface IAzureDeploymentTemplateWriter
    {
        void Write(AzureDeploymentTemplate template, string resourceGroupName);
    }

    public class AzureDeploymentTemplateWriter : IAzureDeploymentTemplateWriter
    {
        private readonly string _folder;

        public AzureDeploymentTemplateWriter(string folder)
        {
            _folder = folder;
        }

        public void Write(AzureDeploymentTemplate template, string resourceGroupName)
        {
            var path = Path.Combine(_folder, resourceGroupName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(Path.Combine(path, "azuredeploy.json"), template.ToString());
            File.WriteAllText(Path.Combine(path, "azuredeploy.parameters.json"), template.Parameters.ToString());
        }
    }

    public class InfrastructureToTemplateJsonRenderer : AzureInfrastructureRenderer
    {
        private readonly IAzureDeploymentTemplateWriter _templateWriter;

        public InfrastructureToTemplateJsonRenderer(
            IAzureDeploymentTemplateWriter templateWriter,
            IResourceGroupTargetingStrategy resourceGroupTargetingStrategy, 
            IResourceLocationTargetingStrategy resourceLocationTargetingStrategy,
            IAzureInfrastructureEnvironment environment, 
            TinyIoCContainer ioc) : base(resourceGroupTargetingStrategy, resourceLocationTargetingStrategy, environment, ioc)
        {
            _templateWriter = templateWriter;
        }

        protected override string TemplateDeploymentVersion(string resourceGroupName)
        {
            return "1.0.0.0";
        }

        protected override void BeforeRender()
        {
        }

        protected override void AfterRender()
        {
        }

        protected override Task BeforeDeployInfrastructure(string resourceGroupName, string location)
        {
            return Task.CompletedTask;
        }

        protected override Task DeployInfrastructure(string resourceGroupName, string location, AzureDeploymentTemplate template)
        {
            _templateWriter.Write(template, resourceGroupName);
            return Task.CompletedTask;
        }

        protected override Task AfterDeployInfrastructure(string resourceGroupName, string location,
            IHaveInfrastructure[] elementsWithInfrastructure)
        {
            return Task.CompletedTask;
        }
    }
}