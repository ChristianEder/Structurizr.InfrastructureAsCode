using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Microsoft.Azure.Management.AppService.Fluent.WebAppBase.Update;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.ARM.Configuration;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{

    public class AppServiceRenderer : AzureResourceRenderer<AppService>
    {
        protected override void Render(
            AzureDeploymentTemplate template,
            IHaveInfrastructure<AppService> container,
            IAzureInfrastructureEnvironment environment,
            string resourceGroup,
            string location)
        {
            template.Resources.Add(new JObject
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
            });

            template.Resources.Add(new JObject
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
            });
        }

        protected override IEnumerable<ConfigurationValue> GetConfigurationValues(IHaveInfrastructure<AppService> elementWithInfrastructure)
        {
            return elementWithInfrastructure.Infrastructure.Settings.Values.Concat(elementWithInfrastructure.Infrastructure.ConnectionStrings.Values);
        }

        protected override async Task Configure(IHaveInfrastructure<AppService> elementWithInfrastructure, AzureConfigurationValueResolverContext context)
        {
            var webapp = await context.Azure.WebApps.GetByResourceGroupAsync(context.ResourceGroupName, elementWithInfrastructure.Infrastructure.Name);

            var appSettings = new Dictionary<string, string>();

            foreach (var setting in elementWithInfrastructure.Infrastructure.Settings)
            {
                object value;
                if (context.Values.TryGetValue(setting.Value, out value))
                {
                    appSettings.Add(setting.Name, value.ToString());
                }
            }

            IUpdate<IWebApp> update = null;
            if (appSettings.Any())
            {
                update = webapp.Update().WithAppSettings(appSettings);

            }

            foreach (var connectionString in elementWithInfrastructure.Infrastructure.ConnectionStrings)
            {
                object value;
                if (context.Values.TryGetValue(connectionString.Value, out value))
                {
                    update = (update ?? webapp.Update()).WithConnectionString(
                        connectionString.Name,
                        value.ToString(),
                        (ConnectionStringType)Enum.Parse(typeof(ConnectionStringType), connectionString.Type));
                }
            }

            if (update != null)
            {
                await update.ApplyAsync();
            }
        }
    }
}