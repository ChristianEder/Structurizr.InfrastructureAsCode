using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public abstract class AppServiceRenderer<TAppService> : AzureResourceRenderer<TAppService>
        where TAppService : AppService
    {
        protected void AddDependsOn(IHaveInfrastructure<AppService> elementWithInfrastructure, JObject template)
        {
            var dependsOn = DependsOn(elementWithInfrastructure);
            if (dependsOn.Any())
            {
                template["dependsOn"] = new JArray(dependsOn);
            }
        }

        protected virtual IEnumerable<string> DependsOn(IHaveInfrastructure<AppService> elementWithInfrastructure)
        {
            var appService = elementWithInfrastructure.Infrastructure;
            var dependsOn = appService.Settings.Values.OfType<IDependentConfigurationValue>()
                .Concat(appService.ConnectionStrings.Values.OfType<IDependentConfigurationValue>())
                .Distinct()
                .Select(s => s.DependsOn.ResourceIdReference);
            return dependsOn;
        }

        protected virtual JObject Properties(IHaveInfrastructure<AppService> elementWithInfrastructure)
        {
            var properties = new JObject { ["name"] = elementWithInfrastructure.Infrastructure.Name };
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