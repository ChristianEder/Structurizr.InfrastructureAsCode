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
    public class WebAppServiceRenderer : AppServiceRenderer<WebAppService>
    {
        protected override void Render(
            AzureDeploymentTemplate template,
            IHaveInfrastructure<WebAppService> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment,
            string resourceGroup,
            string location)
        {
            var name = elementWithInfrastructure.Infrastructure.Name;

            template.Resources.Add(new JObject
            {
                ["type"] = "Microsoft.Web/serverfarms",
                ["name"] = name,
                ["apiVersion"] = "2016-09-01",
                ["location"] = ToLocationName(location),
                ["sku"] = new JObject
                {
                    ["Tier"] = "Free",
                    ["Name"] = "F1"
                },
                ["properties"] = new JObject
                {
                    ["name"] = name,
                    ["workerSizeId"] = "0",
                    ["numberOfWorkers"] = "1",
                    ["reserved"] = false,
                    ["hostingEnvironment"] = ""
                }
            });

            var appService = new JObject
            {
                ["type"] = "Microsoft.Web/sites",
                ["name"] = name,
                ["apiVersion"] = "2015-08-01",
                ["location"] = location,
                ["tags"] = new JObject
                {
                    [
                        $"[concat(\'hidden-related:\', resourceGroup().id, \'/providers/Microsoft.Web/serverfarms/\', \'{name}\')]"
                    ] = "empty"
                },
                ["properties"] = Properties(elementWithInfrastructure)
            };
            AddDependsOn(elementWithInfrastructure, appService);
            template.Resources.Add(appService);
        }

        protected override IEnumerable<string> DependsOn(IHaveInfrastructure<AppService> elementWithInfrastructure)
        {
            return base.DependsOn(elementWithInfrastructure).Concat(Enumerable.Repeat($"[concat(\'Microsoft.Web/serverfarms/\', \'{elementWithInfrastructure.Infrastructure.Name}\')]", 1));
        }


        protected override JObject Properties(IHaveInfrastructure<AppService> elementWithInfrastructure)
        {
            var properties = base.Properties(elementWithInfrastructure);
            properties["serverFarmId"] =
                $"[concat(resourceGroup().id, \'/providers/Microsoft.Web/serverfarms/\', \'{elementWithInfrastructure.Infrastructure.Name}\')]";
            properties["hostingEnvironment"] = "";
            return properties;
        }

        protected override IEnumerable<IConfigurationValue> GetConfigurationValues(IHaveInfrastructure<WebAppService> elementWithInfrastructure)
        {
            return elementWithInfrastructure.Infrastructure.Settings.Values.Concat(elementWithInfrastructure.Infrastructure.ConnectionStrings.Values);
        }

        protected override async Task Configure(IHaveInfrastructure<WebAppService> elementWithInfrastructure, AzureConfigurationValueResolverContext context)
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