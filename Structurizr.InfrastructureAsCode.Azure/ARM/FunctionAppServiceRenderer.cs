using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class FunctionAppServiceRenderer : AppServiceRenderer<FunctionAppService>
    {
        protected override void Render(AzureDeploymentTemplate template,
            IHaveInfrastructure<FunctionAppService> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {

            var appServicePlan = AppServicePlan(elementWithInfrastructure, location);
            if (appServicePlan != null)
            {
                template.Resources.Add(PostProcess(appServicePlan));
            }

            var functionApp = Template(
                "Microsoft.Web/sites",
                elementWithInfrastructure.Infrastructure.Name,
                location,
                ApiVersion
            );

            if (appServicePlan != null)
            {
                AddHiddenRelatedToAppServicePlan(elementWithInfrastructure, functionApp);
            }

            functionApp["properties"] = Properties(elementWithInfrastructure);
            AddSubResources(elementWithInfrastructure, functionApp);

            AddDependsOn(elementWithInfrastructure, location, functionApp);
            AddIdentity(elementWithInfrastructure, functionApp);
            functionApp["kind"] = "functionapp";

            template.Resources.Add(PostProcess(functionApp));
        }

        protected override JObject AppServicePlan(IHaveInfrastructure<FunctionAppService> elementWithInfrastructure, string location)
        {
            return null;
        }

        protected override JObject Properties(IHaveInfrastructure<AppService> elementWithInfrastructure)
        {
            var properties = base.Properties(elementWithInfrastructure);
            properties["clientAffinityEnabled"] = false;
            return properties;
        }
    }
}