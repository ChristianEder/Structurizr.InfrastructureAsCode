using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class NoSqlDocumentDatabaseRenderer : AzureResourceRenderer<NoSqlDocumentDatabase>
    {
        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<NoSqlDocumentDatabase> elementWithInfrastructure, IAzureInfrastructureEnvironment environment, string resourceGroup,
            string location)
        {
            template.Resources.Add(new JObject
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
                            ["id"] = $"{elementWithInfrastructure.Infrastructure.Name}-{location}",
                            ["failoverPriority"] = 0,
                            ["locationName"] = ToLocationName(location)
                        }
                    }
                }
            });
        }
    }
}