using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class MonkeyCrmConnector : ContainerWithInfrastructure<ServiceBus>
    {
        public MonkeyCrmConnector(MonkeyFactory monkeyFactory, IInfrastructureEnvironment environment)
        {
            Container = monkeyFactory.System.AddContainer(
                name: "Monkey CRM Connector",
                description: "Serves as communication channel between the monkey factory cloud and the on premise CRM system",
                technology: "Azure Service Bus");

            Infrastructure = new ServiceBus
            {
                Name = "monkey-crm-service-bus-" + environment.Name,
                EnvironmentInvariantName = "monkey-crm-service-bus"
            };
        }
    }
}