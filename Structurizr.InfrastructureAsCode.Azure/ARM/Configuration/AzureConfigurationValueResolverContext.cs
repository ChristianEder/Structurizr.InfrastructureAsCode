using System.Collections.Generic;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.Fluent;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class AzureConfigurationValueResolverContext
    {
        public IAzure Azure { get; }
        public string ResourceGroupName { get; }

        public AzureConfigurationValueResolverContext(IAzure azure, string resourceGroupName)
        {
            Azure = azure;
            ResourceGroupName = resourceGroupName;
            Values = new Dictionary<ConfigurationValue, object>();
        }


        public Dictionary<ConfigurationValue, object> Values { get; set; }
    }
}