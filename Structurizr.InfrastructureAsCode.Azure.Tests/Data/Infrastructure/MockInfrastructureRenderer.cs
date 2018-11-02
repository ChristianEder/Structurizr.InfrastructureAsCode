using System.Collections.Generic;
using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using TinyIoC;

namespace Structurizr.InfrastructureAsCode.Azure.Tests.Data.Infrastructure
{
    public class MockInfrastructureRenderer : AzureInfrastructureRenderer
    {
        public MockInfrastructureRenderer(TinyIoCContainer ioc) : base(null, null, null, ioc)
        {
        }

        public TinyIoCContainer IoC => Ioc;

        protected override string TemplateDeploymentVersion(string resourceGroupName)
        {
            throw new System.NotImplementedException();
        }

        protected override void BeforeRender()
        {
            throw new System.NotImplementedException();
        }

        protected override Task BeforeDeployInfrastructure(string resourceGroupName, string location)
        {
            throw new System.NotImplementedException();
        }

        protected override Task DeployInfrastructure(string resourceGroupName, string location, AzureDeploymentTemplate template)
        {
            throw new System.NotImplementedException();
        }

        protected override Task AfterDeployInfrastructure(string resourceGroupName, string location, List<IHaveInfrastructure> elementsWithInfrastructure)
        {
            throw new System.NotImplementedException();
        }

        protected override void AfterRender()
        {
            throw new System.NotImplementedException();
        }
    }
}