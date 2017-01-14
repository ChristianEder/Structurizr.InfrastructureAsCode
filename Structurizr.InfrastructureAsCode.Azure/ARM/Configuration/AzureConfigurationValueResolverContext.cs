using System.Collections.Generic;
using Microsoft.Azure.Management.AppService.Fluent;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class AzureConfigurationValueResolverContext
    {
        public IAppServiceManager AppServiceManager { get; }
        public string ResourceGroupName { get; }

        public AzureConfigurationValueResolverContext(IAppServiceManager appServiceManager, string resourceGroupName)
        {
            AppServiceManager = appServiceManager;
            ResourceGroupName = resourceGroupName;
            Values = new Dictionary<ConfigurationValue, object>();
        }


        public Dictionary<ConfigurationValue, object> Values { get; set; }
    }
}