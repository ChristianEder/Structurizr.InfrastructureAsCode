using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class StreamAnalytics : ContainerWithInfrastructure<Structurizr.InfrastructureAsCode.Azure.Model.StreamAnalytics>
    {
        public StreamAnalytics(IotReferenceArchitectureWithFunctions iotReferenceArchitectureWithFunctions, CloudGateway cloudGateway, TelemetryStorage telemetryStorage, IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchitectureWithFunctions.System.AddContainer(
                name: "Stream Analytics",
                description: "Analyzes the incoming event stream for anomalies",
                technology: "Azure Stream Analytics");

            Infrastructure = new Structurizr.InfrastructureAsCode.Azure.Model.StreamAnalytics
            {
                Name = "ref-sa-" + environment.Name
            };

            Uses(cloudGateway).Over(Infrastructure.IotHubInput("iothub", cloudGateway.Infrastructure)).InOrderTo("Process incoming events");
            Uses(telemetryStorage).Over(Infrastructure.BlobStorageInput("blob", telemetryStorage.Infrastructure, "streamingInput")).InOrderTo("Join reference data");
            Uses(telemetryStorage).Over(Infrastructure.TableStorageOutput("out", telemetryStorage.Infrastructure, "aggregated", "partitionKey", "rowKey")).InOrderTo("Store aggregated data");

            Infrastructure.TransformationQuery = "SELECT\r\n    *\r\nINTO\r\n    out\r\nFROM\r\n    iothub";
        }
    }
}