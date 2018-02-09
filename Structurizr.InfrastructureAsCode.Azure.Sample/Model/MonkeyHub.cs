using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
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
}