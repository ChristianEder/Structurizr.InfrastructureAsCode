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

            template.Resources.Add(PostProcess(AppServicePlan(elementWithInfrastructure, location)));

            var appService = new JObject
            {
                ["type"] = "Microsoft.Web/sites",
                ["name"] = name,
                ["apiVersion"] = ApiVersion,
                ["location"] = location,
                ["properties"] = Properties(elementWithInfrastructure),
            };

            AddHiddenRelatedToAppServicePlan(elementWithInfrastructure, appService);
            AddSubResources(elementWithInfrastructure, appService);
            AddDependsOn(elementWithInfrastructure, location, appService);
            AddIdentity(elementWithInfrastructure, appService);
            template.Resources.Add(PostProcess(appService));
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