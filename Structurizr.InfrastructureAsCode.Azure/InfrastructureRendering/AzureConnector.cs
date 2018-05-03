using System.Security.Cryptography.X509Certificates;
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

            //AADSTS70002: Error validating credentials.AADSTS50012: Client is public so a 'client_assertion' should not be presented.
            //Trace ID: bef98c34-7bf4-498d-923c-acd209da0900
            //Correlation ID: 21054e4a-e17a-4513-a08c-99a3f14f5616
            //Timestamp: 2018-05-03 13:38:49Z

            return graph;

        }

        private AzureCredentials AzureCredentials => new AzureCredentialsFactory().FromServicePrincipal(
                _subscriptionCredentials.ClientId,
                Certificate,
                _subscriptionCredentials.TenantId,
                AzureEnvironment.AzureGlobalCloud)
            .WithDefaultSubscription(_subscriptionCredentials.SubscriptionId);

        private X509Certificate2 Certificate
        {
            get
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                try
                {
                    store.Open(OpenFlags.ReadOnly);
                    X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindByThumbprint,
                        _subscriptionCredentials.Thumbprint, false); // Don't validate certs, since the test root isn't installed.
                    return col.Count == 0 ? null : col[0];
                }
                finally
                {
                    store.Close();
                }
            }
        }
    }
}