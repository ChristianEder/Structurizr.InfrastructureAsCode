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
    public class IotReferenceArchHub : ContainerWithInfrastructure<IoTHub>
    {
        public IotReferenceArchHub(IotReferenceArchModel iotReferenceArchModel, IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchModel.System.AddContainer(
                name: "Iot Hub",
                description: "Receives incoming messages from the device",
                technology: "Azure IoT Hub");

            Infrastructure = new IoTHub
            {
                Name = "ref-hub-" + environment.Name,
                EnvironmentInvariantName = "ref-hub"
            };
        }
    }

}
