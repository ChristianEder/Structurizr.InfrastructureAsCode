﻿using System.Linq;
using Microsoft.Extensions.Configuration;
using Structurizr.Client;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Sample.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var workspace = ArchitectureModel();
            RenderInfrastructure(workspace, "dev");
            if (args.Length == 2 && args[0] == "infrastructure")
            {
                RenderInfrastructure(workspace, args[1]);
            }
            else
            {
                UploadToStructurizr(workspace);
            }
        }

        private static void UploadToStructurizr(Workspace workspace)
        {
            var configuration = Configuration();
            var client = new StructurizrClient(configuration["Structurizr:Key"], configuration["Structurizr:Secret"]);
            client.PutWorkspace(int.Parse(configuration["Structurizr:WorkspaceId"]), workspace);
        }

        private static void RenderInfrastructure(Workspace workspace, string environment)
        {
            var renderer = Renderer();
            renderer.Render(workspace.Model, new InfrastructureEnvironment(environment)).Wait();
        }

        private static InfrastructureRenderer Renderer()
        {
            var configuration = Configuration();

            // In order for this to run, you need to create an Azure AD application for this tool first, then configure the AD Apps credentials in the configuration file
            // How to: https://msdn.microsoft.com/en-us/library/mt603580.aspx
            // $app = New-AzureRmADApplication -DisplayName "{appname}" -HomePage "http://{appname}.com" -IdentifierUris "http://{appname}.{tenant name}.onmicrosoft.com" -Password "{app secret}" -EndDate $until
            // New-AzureRmADServicePrincipal -ApplicationId $app.ApplicationId
            // New-AzureRmRoleAssignment -RoleDefinitionName Contributor -ServicePrincipalName $app.ApplicationId

            return new InfrastructureRenderer(
                new ResourceGroupPerEnvironmentStrategy(e => $"shop-{e.Name}"),
                new FixedResourceLocationTargetingStrategy("westeurope"),
                configuration["Azure:ClientId"],
                configuration["Azure:ClientSecret"],
                configuration["Azure:TenantId"],
                configuration["Azure:SubscriptionId"]
                );
        }

        private static Workspace ArchitectureModel()
        {
            var workspace = new Workspace("Shop architecture", "Some generic web implemented with Azure cloud infrastructure");

            var shop = new Shop();
            workspace.Model.Add(shop);
            shop.Initialize();

            var containerView = workspace.Views.CreateContainerView(shop, "Shop Container View", "Overview over the shop system");
            containerView.AddAllContainers();
            return workspace;
        }
        private static IConfigurationRoot Configuration()
        {
            return new ConfigurationBuilder().AddJsonFile(
                "appsettings.json.user",
                optional: false, 
                reloadOnChange: false)
                .Build();
        }
    }
}
