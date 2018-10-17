using System;
using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class DeviceProvisioningService : ContainerInfrastructure, IHaveResourceId, IContainerConnector
    {
        public string ApiVersion = "2017-08-21-preview";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.Devices/provisioningServices', '{Name}')";

        public IList<IoTHub> IotHubs { get; } = new List<IoTHub>();

        void IContainerConnector.Connect<TUsing, TUsed>(ContainerWithInfrastructure<TUsing> usingContainer,
            ContainerWithInfrastructure<TUsed> usedContainer)
        {
            if (!ReferenceEquals(this, usingContainer.Infrastructure))
            {
                throw new InvalidOperationException();
            }

            var ioTHub = usedContainer.Infrastructure as IoTHub;

            if (ReferenceEquals(null, ioTHub))
            {
                throw new InvalidOperationException();
            }


            IotHubs.Add(ioTHub);
        }

        string IContainerConnector.Technology => "";

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
    }
}