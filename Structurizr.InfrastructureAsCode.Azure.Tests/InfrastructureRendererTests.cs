using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Moq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Tests.Data;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using TinyIoC;
using Xunit;

namespace Structurizr.InfrastructureAsCode.Azure.Tests
{
    public class InfrastructureRendererTests
    {
        [Fact]
        public async Task ShouldNotRenderAnythingWhenHavingNoAppropriateRenderer()
        {
            var azure = GetAzure();

            var renderer = GivenARenderer(azure);

            await WhenRendering(renderer);

            azure.Verify(a => a.AppServices.ResourceManager.Deployments.Define(It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public void ShouldGroupResourceDeploymentsByLocation()
        {

        }

        [Fact]
        public void ShouldGroupResourceDeploymentsByResourceGroup()
        {

        }


        [Fact]
        public void ShouldNotConfigureAnythingWhenHavingNoAppropriateConfigurationResolver()
        {

        }


        [Fact]
        public void ShouldConfigureEverythingWhenHavingAppropriateConfigurationResolvers()
        {

        }

        private static InfrastructureRenderer GivenARenderer(Mock<IAzure> azure)
        {
            var renderer = new InfrastructureRenderer(
                new ResourceGroupPerEnvironmentStrategy(e => "test"),
                new FixedResourceLocationTargetingStrategy("testlocation"),
                GetAzureConnector(azure.Object),
                Environment,
                new TinyIoCContainer());
            return renderer;
        }

        private static async Task WhenRendering(InfrastructureRenderer renderer)
        {
            await renderer.Render(new SampleSystem(new Workspace("sample", "sample"), new InfrastructureEnvironment("test")));
        }

        private static IAzureInfrastructureEnvironment Environment
        {
            get
            {
                var mock = new Mock<IAzureInfrastructureEnvironment>();
                mock.Setup(e => e.Name).Returns("test");
                return mock.Object;
            }
        }

        private static IAzureConnector GetAzureConnector(IAzure azure)
        {
            var mock = new Mock<IAzureConnector>();
            mock.Setup(c => c.Azure()).Returns(() => azure);
            mock.Setup(c => c.Graph()).Returns(Mock.Of<IGraphRbacManagementClient>);
            return mock.Object;
        }

        private static Mock<IAzure> GetAzure()
        {
            var mock = new Mock<IAzure>();

            var deployment = new Mock<IDeployment>();
            deployment.Setup(d => d.ProvisioningState).Returns(ProvisioningState.Succeeded.ToString());

            mock.Setup(a => a.AppServices.ResourceManager.ResourceGroups.CheckExistence(It.IsAny<string>())).Returns(true);
            mock.Setup(a => a.Deployments.List()).Returns(new List<IDeployment>());
            mock.Setup(a => a.AppServices.ResourceManager.Deployments
                    .Define(It.IsAny<string>())
                    .WithExistingResourceGroup(It.IsAny<string>())
                    .WithTemplate(It.IsAny<string>())
                    .WithParameters(It.IsAny<object>())
                    .WithMode(DeploymentMode.Incremental)
                    .BeginCreate()
                )
                .Returns(deployment.Object);

            mock.Setup(a => a.AppServices.ResourceManager.Deployments
                    .GetByResourceGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(deployment.Object));

            return mock;
        }
    }
}
