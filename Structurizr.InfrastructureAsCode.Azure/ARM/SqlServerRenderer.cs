using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class SqlServerRenderer : AzureResourceRenderer<SqlServer>
    {
        protected override void Render(AzureDeploymentTemplate template,
            IHaveInfrastructure<SqlServer> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var sqlServer = elementWithInfrastructure.Infrastructure;

            template.Resources.Add(PostProcess(new JObject
            {
                ["type"] = "Microsoft.Sql/servers",
                ["name"] = sqlServer.Name,
                ["apiVersion"] = sqlServer.ApiVersion,
                ["location"] = location,
                ["properties"] = new JObject
                {
                    ["version"] = "12.0",
                    ["administratorLogin"] = sqlServer.AdministratorLogin,
                    ["administratorLoginPassword"] = sqlServer.AdministratorPassword
                },
                ["resources"] = GetResources(sqlServer, location)
            }));
        }

        private JArray GetResources(SqlServer sqlServer, string location)
        {
            var resources = new List<JObject>();

            resources.Add(PostProcess(new JObject
            {
                ["type"] = "firewallrules",
                ["name"] = "AllowAllWindowsAzureIps",
                ["apiVersion"] = sqlServer.ApiVersion,
                ["location"] = location,
                ["properties"] = new JObject
                {
                    ["endIpAddress"] = "0.0.0.0",
                    ["startIpAddress"] = "0.0.0.0"
                },
                ["dependsOn"] = new JArray
                {
                    sqlServer.ResourceIdReference
                }
            }));

            resources.AddRange(
                sqlServer.Databases.Select(database => PostProcess(new JObject
                {
                    ["type"] = "databases",
                    ["name"] = database.Name,
                    ["apiVersion"] = sqlServer.ApiVersion,
                    ["location"] = location,
                    ["properties"] = new JObject
                    {
                        ["collation"] = "SQL_Latin1_General_CP1_CI_AS",
                        ["edition"] = "Basic",
                        ["maxSizeBytes"] = "2147483648",
                        ["zoneRedundant"] = false,
                        ["requestedServiceObjectiveId"] = "dd6d99bb-f193-4ec1-86f2-43d3bccbc49c"
                    },
                    ["dependsOn"] = new JArray
                    {
                        sqlServer.ResourceIdReference
                    }
                })));

            return new JArray(resources);
        }
    }
}