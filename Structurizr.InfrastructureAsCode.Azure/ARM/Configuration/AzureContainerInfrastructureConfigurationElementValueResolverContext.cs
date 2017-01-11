using System.Collections.Generic;
using Microsoft.Azure.Management.Resources;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class AzureContainerInfrastructureConfigurationElementValueResolverContext
    {
        public AzureContainerInfrastructureConfigurationElementValueResolverContext(ResourceManagementClient client)
        {
            Client = client;
            Values = new Dictionary<ContainerInfrastructureConfigurationElementValue, object>();
        }

        public ResourceManagementClient Client { get; }

        public Dictionary<ContainerInfrastructureConfigurationElementValue, object> Values { get; set; }
    }
}