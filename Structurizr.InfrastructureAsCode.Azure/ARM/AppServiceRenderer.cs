using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class AppServiceRenderer : AzureResourceRenderer<AppService>
    {
        protected override IEnumerable<JObject> Render(Container<AppService> container, IAzureInfrastructureEnvironment environment,
            string resourceGroup, string location)
        {
            yield return new JObject
            {
                ["type"] = "Microsoft.Web/serverfarms",
                ["name"] = container.Infrastructure.Name,
                ["apiVersion"] = "2016-09-01",
                ["location"] = ToLocationName(location),
                ["sku"] = new JObject
                {
                    ["Tier"] = "Free",
                    ["Name"] = "F1"
                },
                ["properties"] = new JObject
                {
                    ["name"] = container.Infrastructure.Name,
                    ["workerSizeId"] = "0",
                    ["numberOfWorkers"] = "1",
                    ["reserved"] = false,
                    ["hostingEnvironment"] = ""
                }
            };

            yield return new JObject
            {
                ["type"] = "Microsoft.Web/sites",
                ["name"] = container.Infrastructure.Name,
                ["apiVersion"] = "2015-08-01",
                ["location"] = location,
                ["tags"] = new JObject
                {
                    [
                        $"[concat(\'hidden-related:\', resourceGroup().id, \'/providers/Microsoft.Web/serverfarms/\', \'{container.Infrastructure.Name}\')]"
                    ] = "empty"
                },
                ["properties"] = new JObject
                {
                    ["name"] = container.Infrastructure.Name,
                    ["serverFarmId"] = $"[concat(resourceGroup().id, \'/providers/Microsoft.Web/serverfarms/\', \'{container.Infrastructure.Name}\')]",
                    ["hostingEnvironment"] = ""
                },
                ["dependsOn"] = new JArray
                {
                    $"[concat(\'Microsoft.Web/serverfarms/\', \'{container.Infrastructure.Name}\')]"
                }
            };
        }
    }
}