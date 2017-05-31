using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Structurizr.InfrastructureAsCode.IoC;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    [Injectable]
    public class AzureConnector : IAzureConnector
    {
        private readonly IAzureSubscriptionCredentials _subscriptionCredentials;

        public AzureConnector(IAzureSubscriptionCredentials subscriptionCredentials)
        {
            _subscriptionCredentials = subscriptionCredentials;
        }

        public IAzure Azure()
        {
            return Microsoft.Azure.Management.Fluent.Azure.Authenticate((AzureCredentials) AzureCredentials).WithSubscription(_subscriptionCredentials.SubscriptionId);
        }

        public IGraphRbacManagementClient Graph()
        {
            var graph = new GraphRbacManagementClient(AzureCredentials)
            {
                TenantID = _subscriptionCredentials.TenantId
            };
            return graph;
        }

        private AzureCredentials AzureCredentials => new AzureCredentialsFactory().FromServicePrincipal(
                _subscriptionCredentials.ClientId,
                _subscriptionCredentials.ClientSecret,
                _subscriptionCredentials.TenantId,
                AzureEnvironment.AzureGlobalCloud)
            .WithDefaultSubscription(_subscriptionCredentials.SubscriptionId);
    }
}