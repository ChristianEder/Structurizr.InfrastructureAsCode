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
                "2016-03-01"
            );
            functionApp["properties"] = Properties(elementWithInfrastructure);
            AddDependsOn(elementWithInfrastructure, functionApp);
            functionApp["kind"] = "functionapp";

            template.Resources.Add(functionApp);
        }
        
        protected override JObject Properties(IHaveInfrastructure<AppService> elementWithInfrastructure)
        {
            var properties = base.Properties(elementWithInfrastructure);
            properties["name"] = elementWithInfrastructure.Infrastructure.Name;
            properties["clientAffinityEnabled"] = false;
            return properties;
        }
    }
}