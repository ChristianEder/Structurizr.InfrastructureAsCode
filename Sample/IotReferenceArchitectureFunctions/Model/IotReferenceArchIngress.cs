using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class IotReferenceArchIngress : ContainerWithInfrastructure<FunctionAppService>
    {
        public IotReferenceArchIngress(IotReferenceArchModel iotReferenceArchModel, IotReferenceArchHub hub, 
            IotReferenceArchTelemetryStorage telemetryStorage, IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchModel.System.AddContainer(
                name: "Ingress",
                description: "Receives incomming data from Iot Hub and saves it into SQl and Telemetry store",
                technology: "Azure Function");

            Infrastructure = new FunctionAppService
            {
                Name = "ref-ingress-" + environment.Name
            };

            Uses(hub)
                .Over<IoTHubSDK>()
                .InOrderTo("Subscribes to incoming messages");

            Uses(telemetryStorage)
                .Over(telemetryStorage.Infrastructure.TableEndpoint)
                .InOrderTo("Store telemetry data");
        }
    }
}

