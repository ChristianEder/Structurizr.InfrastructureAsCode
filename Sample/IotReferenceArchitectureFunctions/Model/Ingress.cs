using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class Ingress : ContainerWithInfrastructure<FunctionAppService>
    {
        public Ingress(IotReferenceArchitectureWithFunctions iotReferenceArchitectureWithFunctions, CloudGateway hub,
            TelemetryStorage telemetryStorage,
            MasterDataStorage masterDataStorage,
            SanitizedMessages sanitizedMessages,
            ApplicationInsights applicationInsights, 
            IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchitectureWithFunctions.System.AddContainer(
                name: "Ingress",
                description: "Receives incoming data from the cloud gateway and saves it into master data and telemetry data storages",
                technology: "Azure Function");

            Infrastructure = new FunctionAppService
            {
                Name = "ref-ingress-" + environment.Name
            };

            Uses(hub)
                .Over<IoTHubSDK>()
                .InOrderTo("Subscribes to incoming messages");

            Uses(sanitizedMessages)
                .Over<EventHubSDK>()
                .InOrderTo("Publish sanitized messages");

            Uses(telemetryStorage)
                .Over(telemetryStorage.Infrastructure.TableEndpoint)
                .InOrderTo("Persist telemetry data");

            Uses(masterDataStorage).InOrderTo("Persist master data");

            Uses(applicationInsights).InOrderTo("Log metrics");
        }
    }
}

