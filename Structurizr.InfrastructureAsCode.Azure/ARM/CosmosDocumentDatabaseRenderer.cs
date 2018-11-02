using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class CosmosDocumentDatabaseRenderer : AzureResourceRenderer<CosmosDocumentDatabase>
    {
        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<CosmosDocumentDatabase> elementWithInfrastructure, IAzureInfrastructureEnvironment environment, string resourceGroup,
            string location)
        {
            template.Resources.Add(PostProcess(new JObject
            {
                ["type"] = "Microsoft.DocumentDb/databaseAccounts",
                ["kind"] = "GlobalDocumentDB",
                ["name"] = elementWithInfrastructure.Infrastructure.Name,
                ["apiVersion"] = "2015-04-08",
                ["location"] = location,
                ["properties"] = new JObject
                {
                    ["databaseAccountOfferType"] = "Standard",
                    ["locations"] = new JArray
                    {
                        new JObject
                        {
                            ["failoverPriority"] = 0,
                            ["locationName"] = ToLocationName(location)
                        }
                    }
                }
            }));
        }
    }
}