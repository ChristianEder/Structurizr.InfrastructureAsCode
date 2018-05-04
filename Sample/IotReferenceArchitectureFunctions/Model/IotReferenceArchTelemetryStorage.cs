using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class IotReferenceArchTelemetryStorage : ContainerWithInfrastructure<StorageAccount>
    {
        public IotReferenceArchTelemetryStorage(IotReferenceArchModel iotReferenceArchModel,
            IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchModel.System.AddContainer(
                name: "Telemetry Table Storage",
                description: "Persistently stores all telemetry data from device",
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
