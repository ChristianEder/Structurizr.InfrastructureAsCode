

using System;
using System.IO;
using IotReferenceArchitectureFunctions.Model;
using Microsoft.Extensions.Configuration;
using Structurizr;
using Structurizr.Api;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Policies;

namespace IotReferenceArchitectureFunctions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            UploadToStructurizr();
            RenderInfrastructure("dev");
        }


        private static void UploadToStructurizr()
        {
            var workspace = ArchitectureModel(new InfrastructureEnvironment("prod"));

            var configuration = Configuration();
            var client = new StructurizrClient(configuration["Structurizr:Key"], configuration["Structurizr:Secret"])
            {
                WorkspaceArchiveLocation = null
            };
            client.PutWorkspace(int.Parse(configuration["Structurizr:WorkspaceId"]), workspace);
        }

        private static void RenderInfrastructure(string environmentName)
        {
            var configuration = Configuration();
            var environment = Environment(environmentName, configuration);
            var monkeyFactory = InfrastructureModel(environment);

            var renderer = Renderer(environment, configuration);
            try
            {
                renderer.Render(monkeyFactory).Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null)
                {
                    Console.Error.WriteLine(ex.InnerException.ToString());
                }
                else
                {
                    foreach (var innerException in ex.InnerExceptions)
                    {
                        Console.Error.WriteLine(innerException.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private static IAzureInfrastructureEnvironment Environment(string environment, IConfigurationRoot configuration)
        {
            return new AzureInfrastructureEnvironment(environment, configuration["Azure:TenantId"], configuration["Azure:Administrators"].Split(",".ToCharArray()));
        }

        private static AzureInfrastructureRenderer Renderer(IAzureInfrastructureEnvironment environment, IConfiguration configuration)
        {
            return new InfrastructureRendererBuilder<InfrastructureToResourcesRenderer>()
                .In(environment)
                .UsingResourceGroupPerEnvironment(e => $"iot-rf-{e.Name}")
                .UsingLocation("westeurope")
                .Using<IPasswordPolicy, RandomPasswordPolicy>()
                .UsingCredentials(
                    new AzureSubscriptionCredentials(
                    configuration["Azure:ClientId"],
                    configuration["Azure:ApplicationId"],
                    configuration["Azure:Thumbprint"],
                    configuration["Azure:TenantId"],
                    configuration["Azure:SubscriptionId"]))
                .Build();
        }

        private static Workspace ArchitectureModel(IInfrastructureEnvironment environment)
        {
            var workspace = CreateWorkspace();

            var iotReferenceArchModel = new IotReferenceArchitectureWithFunctions(workspace, environment);

            var contextView = workspace.Views.CreateSystemContextView(iotReferenceArchModel.System, "Iot reference architecture with functions Context view", "Overview over the IoT reference architecture");
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            var containerView = workspace.Views.CreateContainerView(iotReferenceArchModel.System, "Iot reference architecture with functions Container View", "Overview over the IoT reference architecture");

            containerView.AddAllContainers();
            containerView.AddAllPeople();

            foreach (var systemContainer in iotReferenceArchModel.System.Containers)
            {
                containerView.AddNearestNeighbours(systemContainer);
            }

            return workspace;
        }

        private static IotReferenceArchitectureWithFunctions InfrastructureModel(IAzureInfrastructureEnvironment environment)
        {
            return new IotReferenceArchitectureWithFunctions(CreateWorkspace(), environment);
        }

        private static Workspace CreateWorkspace()
        {
            var workspace = new Workspace("Iot Reference architecture with functions", "");
            workspace.Views.Configuration.Styles.Add(new ElementStyle(Tags.Person) { Shape = Shape.Person });
            return workspace;
        }

        private static IConfigurationRoot Configuration()
        {
            return new ConfigurationBuilder().AddJsonFile(
                    Path.Combine("appsettings.json.user"),
                    optional: false,
                    reloadOnChange: false)
                .Build();
        }
    }
}
