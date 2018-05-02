using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class DeviceProvisioningService : ContainerInfrastructure, IHaveResourceId
    {
        public DeviceProvisioningService()
        {
        }

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.Devices/provisioningServices', '{Name}')";

        public string ApiVersion = "2017-08-21-preview";

    }
}
