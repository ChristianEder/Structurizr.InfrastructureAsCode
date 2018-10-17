namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class AzureSubscriptionCredentials : IAzureSubscriptionCredentials
    {
        public AzureSubscriptionCredentials(string clientId, string applicationId, string thumbprint, string tenantId, string subscriptionId)
        {
            ClientId = clientId;
            ApplicationId = applicationId;
            Thumbprint = thumbprint;
            TenantId = tenantId;
            SubscriptionId = subscriptionId;
        }

        public string ClientId { get; }
        public string ApplicationId { get; }
        public string Thumbprint { get; }
        public string TenantId { get; }
        public string SubscriptionId { get; }
    }
}