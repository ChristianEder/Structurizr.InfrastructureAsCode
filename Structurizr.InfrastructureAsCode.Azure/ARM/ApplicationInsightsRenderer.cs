using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class ApplicationInsightsRenderer : AzureResourceRenderer<ApplicationInsights>
    {
        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<ApplicationInsights> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var tags = new JObject();
            foreach (var usedBy in elementWithInfrastructure.Infrastructure.UsedBy)
            {
                tags[usedBy.HiddenLink] = "Resource";
            }

            var insights = Template(
                "microsoft.insights/components",
                elementWithInfrastructure.Infrastructure.Name,
                location);
            insights["tags"] = tags;

            insights["properties"] = new JObject
            {
                ["ApplicationId"] = elementWithInfrastructure.Infrastructure.Name
            };

            template.Resources.Add(PostProcess(insights));
        }
    }
}