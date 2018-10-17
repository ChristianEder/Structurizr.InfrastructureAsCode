using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class MonkeyUI : ContainerWithInfrastructure<WebAppService>
    {
        public MonkeyUI(MonkeyFactory monkeyFactory, MonkeyEventStore eventStore, IInfrastructureEnvironment environment)
        {
            Container = monkeyFactory.System.AddContainer(
                name: "Monkey UI",
                description: "Visualizes the monkey factory data",
                technology: "Azure Web App Service");

            Infrastructure = new WebAppService
            {
                Name = "monkey-ui-" + environment.Name
            };

            Uses(eventStore)
                .Over(eventStore.Infrastructure.TableEndpoint)
                .InOrderTo("Load information about the monkey factory");
        }
    }
}