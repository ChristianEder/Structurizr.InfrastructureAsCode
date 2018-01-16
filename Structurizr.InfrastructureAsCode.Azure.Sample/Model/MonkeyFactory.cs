using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class MonkeyFactory : SoftwareSystemWithInfrastructure
    {
        public MonkeyHub Hub { get; }
        public MonkeyMessageProcessor MessageProcessor { get; }

        public MonkeyFactory(Workspace workspace, IInfrastructureEnvironment environment)
        {
            System = workspace.Model.AddSoftwareSystem(
                Location.Internal, 
                "Monkey Factory",
                "Azure cloud based backend for processing data created during production of monkeys");

            Hub = new MonkeyHub(this, environment);
            MessageProcessor = new MonkeyMessageProcessor(this, Hub, environment);
        }
    }

    public class MonkeyHub : ContainerWithInfrastructure<IoTHub>
    {
        public MonkeyHub(MonkeyFactory monkeyFactory, IInfrastructureEnvironment environment)
        {
            Container = monkeyFactory.System.AddContainer(
                name: "Monkey Hub", 
                description: "Receives incoming messages from the monkey factory production systems", 
                technology: "Azure IoT Hub");

            Infrastructure = new IoTHub
            {
                Name = "monkey-hub-" + environment.Name,
                EnvironmentInvariantName = "monkey-hub"
            };
        }
    }

    public class MonkeyMessageProcessor : ContainerWithInfrastructure<FunctionAppService>
    {
        public MonkeyMessageProcessor(MonkeyFactory monkeyFactory, MonkeyHub hub, IInfrastructureEnvironment environment)
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
        }
    }
}
