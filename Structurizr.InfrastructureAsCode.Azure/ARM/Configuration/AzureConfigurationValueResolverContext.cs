using System.Collections.Generic;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class AzureConfigurationValueResolverContext
    {
        public IAzure Azure { get; }
        public IGraphRbacManagementClient Graph { get; }
        public string ResourceGroupName { get; }

        public AzureConfigurationValueResolverContext(IAzure azure, IGraphRbacManagementClient graph, string resourceGroupName)
        {
            Azure = azure;
            Graph = graph;
            ResourceGroupName = resourceGroupName;
            Values = new Dictionary<IConfigurationValue, object>();
        }


        public Dictionary<IConfigurationValue, object> Values { get; set; }
    }
}