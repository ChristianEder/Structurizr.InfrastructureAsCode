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
            var functionApp = Template(
                "Microsoft.Web/sites",
                elementWithInfrastructure.Infrastructure.Name,
                location,
                ApiVersion
            );
            functionApp["properties"] = Properties(elementWithInfrastructure);

            AddSubResources(elementWithInfrastructure, functionApp);

            AddDependsOn(elementWithInfrastructure, functionApp);
            AddIdentity(elementWithInfrastructure, functionApp);
            functionApp["kind"] = "functionapp";

            template.Resources.Add(functionApp);
        }
        
        protected override JObject Properties(IHaveInfrastructure<AppService> elementWithInfrastructure)
        {
            var properties = base.Properties(elementWithInfrastructure);
            properties["clientAffinityEnabled"] = false;
            return properties;
        }
    }
}