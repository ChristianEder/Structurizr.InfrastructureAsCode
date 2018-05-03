namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class AzureSubscriptionCredentials : IAzureSubscriptionCredentials
    {
        public AzureSubscriptionCredentials(string clientId, string thumbprint, string tenantId, string subscriptionId)
        {
            ClientId = clientId;
            Thumbprint = thumbprint;
            TenantId = tenantId;
            SubscriptionId = subscriptionId;
        }

        public string ClientId { get; }
        public string Thumbprint { get; }
        public string TenantId { get; }
        public string SubscriptionId { get; }
    }
}