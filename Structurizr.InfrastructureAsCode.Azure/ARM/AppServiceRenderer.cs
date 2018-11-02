using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public abstract class AppServiceRenderer<TAppService> : AzureResourceRenderer<TAppService>
        where TAppService : AppService
    {
        protected virtual JObject AppServicePlan(IHaveInfrastructure<TAppService> elementWithInfrastructure,
            string location)
        {
            return new JObject
            {
                ["type"] = "Microsoft.Web/serverfarms",
                ["name"] = elementWithInfrastructure.Infrastructure.Name,
                ["apiVersion"] = ApiVersion,
                ["location"] = ToLocationName(location),
                ["sku"] = new JObject
                {
                    ["Tier"] = "Free",
                    ["Name"] = "F1"
                },
                ["properties"] = new JObject
                {
                    ["name"] = elementWithInfrastructure.Infrastructure.Name,
                    ["workerSizeId"] = "0",
                    ["numberOfWorkers"] = "1",
                    ["reserved"] = false,
                    ["hostingEnvironment"] = ""
                }
            };
        }


        protected override IEnumerable<IConfigurationValue> GetConfigurationValues(IHaveInfrastructure<TAppService> elementWithInfrastructure)
        {
            return elementWithInfrastructure.Infrastructure.Settings.Values.Concat(elementWithInfrastructure.Infrastructure.ConnectionStrings.Values);
        }

        protected void AddDependsOn(IHaveInfrastructure<TAppService> elementWithInfrastructure, string location, JObject template)
        {
            var dependsOn = DependsOn(elementWithInfrastructure, location);
            if (dependsOn.Any())
            {
                template["dependsOn"] = new JArray(dependsOn);
            }
        }

        protected void AddHiddenRelatedToAppServicePlan(IHaveInfrastructure<AppService> elementWithInfrastructure, JObject template)
        {
            template["tags"] = new JObject
            {
                [
                    $"[concat(\'hidden-related:\', resourceGroup().id, \'/providers/Microsoft.Web/serverfarms/\', \'{elementWithInfrastructure.Infrastructure.Name}\')]"
                ] = "empty"
            };
        }

        protected virtual IEnumerable<string> DependsOn(IHaveInfrastructure<TAppService> elementWithInfrastructure, string location)
        {
            var appServicePlan = AppServicePlan(elementWithInfrastructure, location);
            if (appServicePlan != null)
            {
                return Enumerable.Repeat(
                    $"[concat(\'Microsoft.Web/serverfarms/\', \'{elementWithInfrastructure.Infrastructure.Name}\')]",
                    1);
            }

            return Enumerable.Empty<string>();
        }

        protected virtual JObject Properties(IHaveInfrastructure<AppService> elementWithInfrastructure)
        {
            var appService = elementWithInfrastructure.Infrastructure;
            var properties = new JObject { ["name"] = appService.Name };
            return properties;
        }

        protected virtual void AddSubResources(IHaveInfrastructure<AppService> elementWithInfrastructure, JObject appService)
        {
            AppendSettingsResource(elementWithInfrastructure, elementWithInfrastructure.Infrastructure.Settings, appService);
            AppendSettingsResource(elementWithInfrastructure, elementWithInfrastructure.Infrastructure.ConnectionStrings, appService);
        }

        protected virtual void AddIdentity(IHaveInfrastructure<AppService> elementWithInfrastructure, JObject appService)
        {
            if (elementWithInfrastructure.Infrastructure.UseSystemAssignedIdentity)
            {
                appService["identity"] = new JObject
                {
                    ["type"] = "SystemAssigned"
                };
            }
        }

        private void AppendSettingsResource(IHaveInfrastructure<AppService> elementWithInfrastructure, IEnumerable<AppServiceSetting> settings, JObject appService)
        {
            try
            {
                var resolvedSettings = settings.ToArray();
                if (!resolvedSettings.Any())
                {
                    return;
                }

                var isConnectionStrings = resolvedSettings.First() is AppServiceConnectionString;

                var resources = appService["resources"] as JArray;
                if (resources == null)
                {
                    appService["resources"] = resources = new JArray();
                }

                var dependsOn = new JArray
                {
                    elementWithInfrastructure.Infrastructure.Name
                };

                var properties = new JObject();

                var config = new JObject
                {
                    ["apiVersion"] = ApiVersion,
                    ["name"] = isConnectionStrings ? "connectionstrings" : "appsettings",
                    ["type"] = "config",
                    ["dependsOn"] = dependsOn,
                    ["properties"] = properties
                };

                foreach (var setting in resolvedSettings)
                {
                    var value = setting.Value;
                    var dependentValue = value as IDependentConfigurationValue;
                    if (dependentValue != null)
                    {
                        dependsOn.Add(dependentValue.DependsOn.ResourceIdReference);
                    }

                    if (isConnectionStrings)
                    {
                        properties[setting.Name] = new JObject
                        {
                            ["value"]  = JToken.FromObject(setting.Value.Value),
                            ["type"] = ((AppServiceConnectionString) setting).Type
                        };
                    }
                    else
                    {
                        properties[setting.Name] = JToken.FromObject(setting.Value.Value);
                    }
                }

                resources.Add(config);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected const string ApiVersion = "2016-03-01";
    }
}