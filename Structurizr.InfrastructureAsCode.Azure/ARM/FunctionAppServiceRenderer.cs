using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class FunctionAppServiceRenderer : AzureResourceRenderer<FunctionAppService>
    {
        protected override void Render(AzureDeploymentTemplate template,
            IHaveInfrastructure<FunctionAppService> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var functionApp = Template(
                "Microsoft.Web/sites",
                elementWithInfrastructure.Infrastructure.Name,
                location,
                "2016-03-01"
            );
            functionApp["properties"] = Properties(elementWithInfrastructure);
            AddDependsOn(elementWithInfrastructure, functionApp);
            functionApp["kind"] = "functionapp";

            template.Resources.Add(functionApp);
        }

        private static void AddDependsOn(IHaveInfrastructure<FunctionAppService> elementWithInfrastructure, JObject functionApp)
        {
            var appService = elementWithInfrastructure.Infrastructure;
            var dependsOn = appService.Settings.Values.OfType<IDependentConfigurationValue>()
                .Concat(appService.ConnectionStrings.Values.OfType<IDependentConfigurationValue>())
                .Distinct()
                .ToArray();
            if (dependsOn.Any())
            {
                functionApp["dependsOn"] = new JArray(dependsOn.Select(d => d.DependsOn.ResourceIdReference));
            }
        }

        private static JObject Properties(IHaveInfrastructure<FunctionAppService> elementWithInfrastructure)
        {
            var properties = new JObject
            {
                ["name"] = elementWithInfrastructure.Infrastructure.Name,
                ["clientAffinityEnabled"] = false
            };

            Append(elementWithInfrastructure.Infrastructure.Settings, properties, "appSettings");
            Append(elementWithInfrastructure.Infrastructure.ConnectionStrings, properties, "connectionStrings");
            return properties;
        }

        private static void Append(IEnumerable<ConfigurationElement> settings, JObject properties, string settingsType)
        {
            var resolvedSettings = settings
                .Where(s => s.Value.IsResolved)
                .ToArray();
            if (resolvedSettings.Any())
            {
                var appSettings = new JArray();
                foreach (var setting in resolvedSettings)
                {
                    appSettings.Add(new JObject
                    {
                        ["name"] = setting.Name,
                        ["value"] = JToken.FromObject(setting.Value.Value)
                    });
                }

                var siteConfig = properties["siteConfig"] as JObject;
                if (siteConfig == null)
                {
                    properties["siteConfig"] = siteConfig = new JObject();
                }
                siteConfig[settingsType] = appSettings;
            }
        }
    }
}