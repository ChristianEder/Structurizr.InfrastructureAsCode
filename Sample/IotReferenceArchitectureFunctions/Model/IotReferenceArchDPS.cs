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
    public class IotReferenceArchDPS : ContainerWithInfrastructure<DeviceProvisioningService>
    {
        public IotReferenceArchDPS(IotReferenceArchModel iotReferenceArchModel, IotReferenceArchHub hub,
            IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchModel.System.AddContainer(
                name: "DPS",
                description: "Provision devices",
                technology: "Azure Device Provisioning Service");

            Infrastructure = new DeviceProvisioningService()
            {
                Name = "ref-dps-" + environment.Name
            };

            Uses(hub).InOrderTo("Register provisioned Devices");
        }
    }
}
