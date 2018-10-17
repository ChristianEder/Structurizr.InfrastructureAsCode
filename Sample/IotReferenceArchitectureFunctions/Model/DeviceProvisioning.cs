using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class DeviceProvisioning : ContainerWithInfrastructure<DeviceProvisioningService>
    {
        public DeviceProvisioning(IotReferenceArchitectureWithFunctions iotReferenceArchitectureWithFunctions, CloudGateway hub,
            IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchitectureWithFunctions.System.AddContainer(
                name: "Device Provisioning",
                description: "Provision device identities into the cloud gateway",
                technology: "Azure Device Provisioning Service");

            Infrastructure = new DeviceProvisioningService
            {
                Name = "ref-dps-" + environment.Name
            };

            Uses(hub).InOrderTo("Register provisioned Devices");
        }
    }
}
