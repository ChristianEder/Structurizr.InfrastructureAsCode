using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Microsoft.Azure.Management.AppService.Fluent.WebAppBase.Update;
using Newtonsoft.Json.Linq;
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

            template.Resources.Add(PostProcess(new JObject
            {
                ["type"] = "Microsoft.Web/serverfarms",
                ["name"] = name,
                ["apiVersion"] = ApiVersion,
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
            }));

            var appService = new JObject
            {
                ["type"] = "Microsoft.Web/sites",
                ["name"] = name,
                ["apiVersion"] = ApiVersion,
                ["location"] = location,
                ["tags"] = new JObject
                {
                    [
                        $"[concat(\'hidden-related:\', resourceGroup().id, \'/providers/Microsoft.Web/serverfarms/\', \'{name}\')]"
                    ] = "empty"
                },
                ["properties"] = Properties(elementWithInfrastructure),
            };

            AddSubResources(elementWithInfrastructure, appService);
            AddDependsOn(elementWithInfrastructure, appService);
            AddIdentity(elementWithInfrastructure, appService);
            template.Resources.Add(PostProcess(appService));
        }

        protected override IEnumerable<string> DependsOn(IHaveInfrastructure<AppService> elementWithInfrastructure)
        {
            return base.DependsOn(elementWithInfrastructure)
                .Concat(Enumerable.Repeat($"[concat(\'Microsoft.Web/serverfarms/\', \'{elementWithInfrastructure.Infrastructure.Name}\')]", 1));
        }


        protected override JObject Properties(IHaveInfrastructure<AppService> elementWithInfrastructure)
        {
            var properties = base.Properties(elementWithInfrastructure);
            properties["serverFarmId"] =
                $"[concat(resourceGroup().id, \'/providers/Microsoft.Web/serverfarms/\', \'{elementWithInfrastructure.Infrastructure.Name}\')]";
            properties["hostingEnvironment"] = "";
            return properties;
        }
    }
}