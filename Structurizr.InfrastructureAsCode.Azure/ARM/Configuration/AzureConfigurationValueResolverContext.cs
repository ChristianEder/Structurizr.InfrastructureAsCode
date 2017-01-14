using System.Collections.Generic;
using Microsoft.Azure.Management.Resources;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class AzureConfigurationValueResolverContext
    {
        public AzureConfigurationValueResolverContext(ResourceManagementClient client)
        {
            Client = client;
            Values = new Dictionary<ConfigurationValue, object>();
        }

        public ResourceManagementClient Client { get; }

        public Dictionary<ConfigurationValue, object> Values { get; set; }
    }
}