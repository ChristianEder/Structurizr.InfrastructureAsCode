using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent.Models;
using Microsoft.Azure.Management.Resource.Fluent;
using Microsoft.Azure.Management.Resource.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using Structurizr.Client;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.Azure.Sample.Model;
using Structurizr.InfrastructureAsCode.Policies;

namespace Structurizr.InfrastructureAsCode.Azure.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = Configuration();
            var x = new GraphRbacManagementClient(AzureCredentials.FromUser(
                config["Azure:UserName"],
                config["Azure:Password"],
                config["Azure:ClientId"],
                config["Azure:TenantId"],
                AzureEnvironment.AzureGlobalCloud))
            {
                TenantID = config["Azure:TenantId"]
            };


            var applicationCreateParametersInner = new ApplicationCreateParametersInner
            {
                DisplayName = "chedtest",
                Homepage = $"https://chedtest",
                IdentifierUris = new List<string> { $"https://chedtest" },
                PasswordCredentials =
                   new List<PasswordCredential>
                   {
                        new PasswordCredential { EndDate = DateTime.Now.AddYears(1000), Value = "xle324j_sdf2"}
                   }
            };
            try
            {
                var application = x.Applications.CreateAsync(
               applicationCreateParametersInner).Result;
            }
            catch (Exception ex)
            {

                throw;
            }

            HttpClient client = new HttpClient();

            string body = String.Format("resource={0}&grant_type=password&username={1}&password={2}", 
                Uri.EscapeDataString("https://graph.windows.net"),
                //"a4a45fb7-a6f2-4f3c-a710-5be5187e9425", 
                //Uri.EscapeDataString("4fsj3jhsd93ls"), 
                Uri.EscapeDataString("christian.eder@zuehlke.com"), 
                Uri.EscapeDataString("ze6:Delirium17"));

            StringContent sc = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            var resoult = client.PostAsync("https://login.microsoftonline.com/xxx.onmicrosoft.com/oauth2/token", sc).Result.Content.ReadAsStringAsync().Result;





            if (args.Length == 2 && args[0] == "infrastructure")
            {
                var configuration = Configuration();
                var environment = Environment(args[1], configuration);
                var workspace = ArchitectureModel(environment);
                RenderInfrastructure(workspace, environment, Configuration());
            }
            else if (args.Length == 1 && args[0] == "structurizr")
            {
                var workspace = ArchitectureModel(null);
                UploadToStructurizr(workspace);
            }
            else
            {
                Console.WriteLine("You should run this with one of the following parameters:");
                Console.WriteLine("1) infrastructure <environment>");
                Console.WriteLine("2) structurizr");
                Console.ReadLine();
            }
        }

        private static void UploadToStructurizr(Workspace workspace)
        {
            var configuration = Configuration();
            var client = new StructurizrClient(configuration["Structurizr:Key"], configuration["Structurizr:Secret"])
            {
                WorkspaceArchiveLocation = null
            };
            client.PutWorkspace(int.Parse(configuration["Structurizr:WorkspaceId"]), workspace);
        }

        private static void RenderInfrastructure(Workspace workspace, IAzureInfrastructureEnvironment environment,
            IConfigurationRoot configuration)
        {
            var renderer = Renderer(environment, configuration);
            try
            {
                renderer.Render(workspace.Model).Wait();
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

        private static InfrastructureRenderer Renderer(IAzureInfrastructureEnvironment environment, IConfiguration configuration)
        {

            // In order for this to run, you need to create an Azure AD application for this tool first, then configure the AD Apps credentials in the configuration file
            // How to: https://msdn.microsoft.com/en-us/library/mt603580.aspx
            // $app = New-AzureRmADApplication -DisplayName "{appname}" -HomePage "http://{appname}.com" -IdentifierUris "http://{appname}.{tenant name}.onmicrosoft.com" -Password "{app secret}" -EndDate $until
            // New-AzureRmADServicePrincipal -ApplicationId $app.ApplicationId
            // New-AzureRmRoleAssignment -RoleDefinitionName Contributor -ServicePrincipalName $app.ApplicationId

            return new InfrastructureRendererBuilder()
                .In(environment)
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

        private static Workspace ArchitectureModel(IAzureInfrastructureEnvironment environment)
        {
            var workspace = new Workspace("Shop architecture", "Some generic web implemented with Azure cloud infrastructure");

            var shop = new Shop(environment);
            workspace.Model.Add(shop);
            shop.Initialize();

            var contextView = workspace.Views.CreateSystemContextView(shop, "Shop context view", "Overview over the shop system");
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            var containerView = workspace.Views.CreateContainerView(shop, "Shop Container View", "Overview over the shop system architecture");
            containerView.AddAllContainers();
            containerView.AddAllPeople();

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
