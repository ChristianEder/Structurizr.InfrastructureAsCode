namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class AzureSubscriptionCredentials : IAzureSubscriptionCredentials
    {
        public AzureSubscriptionCredentials(string clientId, string clientSecret, string tenantId, string subscriptionId)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            TenantId = tenantId;
            SubscriptionId = subscriptionId;
        }

        public string ClientId { get; }
        public string ClientSecret { get; }
        public string TenantId { get; }
        public string SubscriptionId { get; }
    }
}