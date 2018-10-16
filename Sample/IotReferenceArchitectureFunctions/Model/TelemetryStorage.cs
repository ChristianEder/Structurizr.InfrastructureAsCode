using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class TelemetryStorage : ContainerWithInfrastructure<StorageAccount>
    {
        public TelemetryStorage(IotReferenceArchitectureWithFunctions iotReferenceArchitectureWithFunctions,
            IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchitectureWithFunctions.System.AddContainer(
                name: "Telemetry Storage",
                description: "Stores all telemetry data from the devices",
                technology: "Azure Table Storage");

            Infrastructure = new StorageAccount
            {
                Kind = StorageAccountKind.StorageV2,
                Name = "reftelemetry" + environment.Name,
                EnvironmentInvariantName = "reftelemetry"
            };
        }
    }
}
