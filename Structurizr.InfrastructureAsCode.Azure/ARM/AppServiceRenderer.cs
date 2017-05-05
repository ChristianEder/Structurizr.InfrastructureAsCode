using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.ARM.Configuration;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class AppServiceRenderer : AzureResourceRenderer<AppService>
    {
        protected override IEnumerable<JObject> Render(ContainerWithInfrastructure<AppService> container, IAzureInfrastructureEnvironment environment,
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

        protected override IEnumerable<ConfigurationValue> GetConfigurationValues(ContainerWithInfrastructure<AppService> container)
        {
            return container.Infrastructure.Settings.Values.Concat(container.Infrastructure.ConnectionStrings.Values);
        }

        protected override async Task Configure(ContainerWithInfrastructure<AppService> container, AzureConfigurationValueResolverContext context)
        {
            var webapp = await context.Azure.WebApps.GetByResourceGroupAsync(context.ResourceGroupName, container.Infrastructure.Name);

            var appSettings = new Dictionary<string, string>();

            foreach (var setting in container.Infrastructure.Settings)
            {
                object value;
                if (context.Values.TryGetValue(setting.Value, out value))
                {
                    appSettings.Add(setting.Name, value.ToString());
                }
            }

            var update = webapp.Update().WithAppSettings(appSettings);

            foreach (var connectionString in container.Infrastructure.ConnectionStrings)
            {
                object value;
                if (context.Values.TryGetValue(connectionString.Value, out value))
                {
                    update = update.WithConnectionString(
                        connectionString.Name, 
                        value.ToString(), 
                        (ConnectionStringType) Enum.Parse(typeof(ConnectionStringType), connectionString.Type));
                }
            }

            await update.ApplyAsync();
        }
    }
}