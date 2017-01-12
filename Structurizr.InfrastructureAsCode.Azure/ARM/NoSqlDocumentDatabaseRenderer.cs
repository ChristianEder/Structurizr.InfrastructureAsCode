using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class NoSqlDocumentDatabaseRenderer : AzureResourceRenderer<NoSqlDocumentDatabase>
    {
        protected override IEnumerable<JObject> Render(Container<NoSqlDocumentDatabase> container, IAzureInfrastructureEnvironment environment, string resourceGroup,
            string location)
        {
            yield return new JObject
            {
                ["type"] = "Microsoft.DocumentDb/databaseAccounts",
                ["kind"] = "GlobalDocumentDB",
                ["name"] = container.Infrastructure.Name,
                ["apiVersion"] = "2015-04-08",
                ["location"] = location,
                ["properties"] = new JObject
                {
                    ["databaseAccountOfferType"] = "Standard",
                    ["locations"] = new JArray
                    {
                        new JObject
                        {
                            ["id"] = $"{container.Infrastructure.Name}-{location}",
                            ["failoverPriority"] = 0,
                            ["locationName"] = ToLocationName(location)
                        }
                    }
                }
            };
        }
    }
}