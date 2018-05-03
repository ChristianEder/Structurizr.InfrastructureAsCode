using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class MonkeyDeviceProvisioningService : ContainerWithInfrastructure<DeviceProvisioningService>
    {

        public MonkeyDeviceProvisioningService(MonkeyFactory monkeyFactory, MonkeyHub hub,
            IInfrastructureEnvironment environment)
        {
            Container = monkeyFactory.System.AddContainer(
                name: "Monkey Device Provisioning Service",
                description: "Registeres to Device Provisioning Service", 
                technology: "Azure Device Provisioning Service");

            Infrastructure = new DeviceProvisioningService()
            {
                Name = "monkey-dps-" + environment.Name
            };

            Uses(hub).InOrderTo("Provision Devices");
        }

    }
}
