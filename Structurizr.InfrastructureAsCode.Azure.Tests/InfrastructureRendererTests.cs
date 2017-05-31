using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Microsoft.Azure.Management.AppService.Fluent.WebAppBase.Update;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Moq;
using Structurizr.InfrastructureAsCode.Azure.ARM;
using Structurizr.InfrastructureAsCode.Azure.ARM.Configuration;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.Azure.Tests.Data;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;
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
            var renderer = GivenARenderer(azure.Object, ioc: new TinyIoCContainer());

            await WhenRendering(renderer);

            azure.Verify(a => a.AppServices.ResourceManager.Deployments.Define(It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public async Task ShouldGroupResourceDeploymentsByLocation()
        {
            var azure = GetAzure();
            var renderer = GivenARenderer(azure.Object,
                resourceLocationTargetingStrategy: DistributeToLocations());

            await WhenRendering(renderer);

            azure.Verify(a => a.AppServices.ResourceManager.Deployments.Define(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ShouldGroupResourceDeploymentsByResourceGroup()
        {
            var azure = GetAzure();
            var renderer = GivenARenderer(azure.Object,
                resourceGroupTargetingStrategy: DistributeToResourceGroups());

            await WhenRendering(renderer);

            azure.Verify(a => a.AppServices.ResourceManager.Deployments.Define(It.IsAny<string>()).WithExistingResourceGroup(It.IsAny<string>()), Times.Exactly(2));
            azure.Verify(a => a.AppServices.ResourceManager.Deployments.Define(It.IsAny<string>()).WithExistingResourceGroup("Sample"), Times.Once);
            azure.Verify(a => a.AppServices.ResourceManager.Deployments.Define(It.IsAny<string>()).WithExistingResourceGroup("Sample API"), Times.Once);

        }


        [Fact]
        public async Task ShouldNotConfigureAnythingWhenHavingNoAppropriateConfigurationResolver()
        {
            var webApps = new List<Mock<IWebApp>>();
            var azure = GetAzure();

            SetupWebApp(azure, mock => webApps.Add(mock));

            var renderer = GivenARenderer(azure.Object);

            await WhenRendering(renderer);

            Assert.Equal(2, webApps.Count);
            foreach (var webApp in webApps)
            {
                webApp.Verify(a => a.Update(), Times.Never);
            }
        }


        [Fact]
        public async Task ShouldConfigureEverythingWhenHavingAppropriateConfigurationResolvers()
        {
            var webApps = new List<Mock<IWebApp>>();
            var azure = GetAzure();

            SetupWebApp(azure, mock =>
            {
                webApps.Add(mock);
                SetupWebAppUpdate(mock);
            });

            var renderer = GivenARenderer(azure.Object,
                ioc: WithConfigurationResolvers(WithRenderers(new TinyIoCContainer())));

            await WhenRendering(renderer);

            Assert.Equal(2, webApps.Count);
            foreach (var webApp in webApps)
            {
                if (webApp.Object.Name == "sampleapi")
                {
                    webApp.Verify(a => a.Update(), Times.Never);
                }
                else if (webApp.Object.Name == "sample")
                {
                    webApp.Verify(a => a.Update(), Times.Once);
                    var expectedSettings = new Dictionary<string, string> {{"apiUrl", "sampleapi.azure.com"}};
                    webApp.Verify(a => a.Update().WithAppSettings(expectedSettings), Times.Once);
                }
                else
                {
                    throw new InvalidOperationException("Unkown web app");
                }
            }
        }

        private InfrastructureRenderer GivenARenderer(
            IAzure azure,
            IResourceGroupTargetingStrategy resourceGroupTargetingStrategy = null,
            IResourceLocationTargetingStrategy resourceLocationTargetingStrategy = null,
            TinyIoCContainer ioc = null
            )
        {
            resourceGroupTargetingStrategy = resourceGroupTargetingStrategy ??
                                             new ResourceGroupPerEnvironmentStrategy(e => "test");

            resourceLocationTargetingStrategy = resourceLocationTargetingStrategy ??
                                                 new FixedResourceLocationTargetingStrategy("westeurope");

            ioc = ioc ?? WithRenderers(new TinyIoCContainer());

            var renderer = new InfrastructureRenderer(
                resourceGroupTargetingStrategy,
                resourceLocationTargetingStrategy,
                GetAzureConnector(azure),
                Environment,
                ioc);
            return renderer;
        }

        private IResourceLocationTargetingStrategy DistributeToLocations()
        {
            var mock = new Mock<IResourceLocationTargetingStrategy>();

            mock.Setup(s => s.TargetLocation(
                    It.IsAny<IInfrastructureEnvironment>(),
                    It.IsAny<ContainerWithInfrastructure>()))
                .Returns((IInfrastructureEnvironment e, ContainerWithInfrastructure c) => c is SampleApi ? "northeurope" : "westeurope");

            return mock.Object;
        }

        private IResourceGroupTargetingStrategy DistributeToResourceGroups()
        {
            var mock = new Mock<IResourceGroupTargetingStrategy>();

            mock.Setup(s => s.TargetResourceGroup(
                    It.IsAny<IInfrastructureEnvironment>(),
                    It.IsAny<ContainerWithInfrastructure>()))
                .Returns((IInfrastructureEnvironment e, ContainerWithInfrastructure c) => c.Container.Name);

            return mock.Object;
        }

        private TinyIoCContainer WithRenderers(TinyIoCContainer ioc)
        {
            ioc.Register<AzureResourceRenderer<AppService>, AppServiceRenderer>();
            return ioc;
        }

        private TinyIoCContainer WithConfigurationResolvers(TinyIoCContainer ioc)
        {
            ioc.Register<IConfigurationValueResolver<AppServiceUrl>, AppServiceUrlResolver>();
            return ioc;
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

            SetupWebApp(mock);

            return mock;
        }

        private static void SetupWebApp(Mock<IAzure> mock, Action<Mock<IWebApp>> onWebApp = null)
        {
            onWebApp = onWebApp ?? SetupWebAppUpdate;

            var webApps = new Dictionary<string, Dictionary<string, Mock<IWebApp>>>();

            mock.Setup(a => a.WebApps.GetByResourceGroupAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns((string resourceGroup, string app, CancellationToken c) =>
                {
                    Dictionary<string, Mock<IWebApp>> appsInResourceGroup;
                    if (!webApps.TryGetValue(resourceGroup, out appsInResourceGroup))
                    {
                        appsInResourceGroup = new Dictionary<string, Mock<IWebApp>>();
                        webApps.Add(resourceGroup, appsInResourceGroup);
                    }

                    Mock<IWebApp> webApp;
                    if (!appsInResourceGroup.TryGetValue(app, out webApp))
                    {
                        webApp = new Mock<IWebApp>();
                        webApp.Setup(a => a.Name).Returns(app);
                        webApp.Setup(a => a.DefaultHostName).Returns(() => app + ".azure.com");
                        appsInResourceGroup.Add(app, webApp);
                        onWebApp(webApp);
                    }

                    return Task.FromResult(webApp.Object);
                });
        }

        private static void SetupWebAppUpdate(Mock<IWebApp> webApp)
        {
            webApp.Setup(a => a.Update().WithAppSettings(It.IsAny<IDictionary<string, string>>()))
                .Returns(() =>
                {
                    var update = new Mock<IUpdate<IWebApp>>();
                    update.Setup(u => u.WithConnectionString(It.IsAny<string>(), It.IsAny<string>(),
                            It.IsAny<ConnectionStringType>()))
                        .Returns(update.Object);

                    update.Setup(u => u.ApplyAsync(It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                        .Returns(() => Task.FromResult(webApp.Object));

                    return update.Object;
                });
        }
    }
}
