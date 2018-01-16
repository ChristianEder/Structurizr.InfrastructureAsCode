using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Structurizr.Api;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Sample.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Policies;

namespace Structurizr.InfrastructureAsCode.Azure.Sample
{
   
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "infrastructure")
            {
                RenderInfrastructure(args[1]);
            }
            else if (args.Length == 1 && args[0] == "structurizr")
            {
                UploadToStructurizr();
            }
            else
            {
                Console.WriteLine("You should run this with one of the following parameters:");
                Console.WriteLine("1) infrastructure <environment>");
                Console.WriteLine("2) structurizr");
                Console.ReadLine();
            }
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
            var shop = InfrastructureModel(environment);

            var renderer = Renderer(environment, configuration);
            try
            {
                renderer.Render(shop).Wait();
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

            // In order for this to run, you need to create an Azure AD application for this tool first, then configure the AD Apps credentials in the configuration file
            // How to: https://msdn.microsoft.com/en-us/library/mt603580.aspx
            // $app = New-AzureRmADApplication -DisplayName "{appname}" -HomePage "http://{appname}.com" -IdentifierUris "http://{appname}.{tenant name}.onmicrosoft.com" -Password "{app secret}" -EndDate $until
            // New-AzureRmADServicePrincipal -ApplicationId $app.ApplicationId
            // New-AzureRmRoleAssignment -RoleDefinitionName Contributor -ServicePrincipalName $app.ApplicationId

            return new InfrastructureRendererBuilder<InfrastructureToTemplateJsonRenderer>()
                .In(environment)
                .Using<IAzureDeploymentTemplateWriter>(new AzureDeploymentTemplateWriter("dev-4"))
                .UsingResourceGroupPerEnvironment(e => $"shop-{e.Name}")
                .UsingLocation("westeurope")
                .Using<IPasswordPolicy, RandomPasswordPolicy>()
                .UsingCredentials(
                    new AzureSubscriptionCredentials(
                    configuration["Azure:ClientId"],
                    configuration["Azure:ClientSecret"],
                    configuration["Azure:TenantId"],
                    configuration["Azure:SubscriptionId"]))
                .Build();
        }

        private static Workspace ArchitectureModel(IInfrastructureEnvironment environment)
        {
            var workspace = CreateWorkspace();

            var shop = new Shop(workspace, environment);

            var contextView = workspace.Views.CreateSystemContextView(shop.System, "Shop context view", "Overview over the shop system");
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            var containerView = workspace.Views.CreateContainerView(shop.System, "Shop Container View", "Overview over the shop system architecture");
            containerView.AddAllContainers();
            containerView.AddAllPeople();

            return workspace;
        }

        private static Shop InfrastructureModel(IAzureInfrastructureEnvironment environment)
        {
            return new Shop(CreateWorkspace(), environment);
        }

        private static Workspace CreateWorkspace()
        {
            var workspace = new Workspace("Shop architecture", "Some generic web implemented with Azure cloud infrastructure");
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
