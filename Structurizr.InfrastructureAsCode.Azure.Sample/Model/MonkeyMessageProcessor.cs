using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class MonkeyMessageProcessor : ContainerWithInfrastructure<FunctionAppService>
    {
        public MonkeyMessageProcessor(MonkeyFactory monkeyFactory, MonkeyHub hub, MonkeyCrmConnector crmConnector,
            MonkeyEventStore eventStore, IInfrastructureEnvironment environment)
        {
            Container = monkeyFactory.System.AddContainer(
                name: "Monkey Message Processor",
                description: "Reads incoming messages, checks for alert conditions, stores messages",
                technology: "Azure Function");

            Infrastructure = new FunctionAppService
            {
                Name = "monkey-message-processor-" + environment.Name
            };

            Uses(hub)
                .Over<IoTHubSDK>()
                .InOrderTo("Read incoming messages");

            Uses(crmConnector)
                .Over(crmConnector.Infrastructure.Queue("monkey-alerts"))
                .InOrderTo("Notify the CRM system of production failures");

            Uses(eventStore)
                .Over(eventStore.Infrastructure.TableEndpoint)
                .InOrderTo("Store incoming messages");
        }
    }
}

