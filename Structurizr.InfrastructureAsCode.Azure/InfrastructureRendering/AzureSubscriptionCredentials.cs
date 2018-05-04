namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class AzureSubscriptionCredentials : IAzureSubscriptionCredentials
    {
        public AzureSubscriptionCredentials(string clientId, string applicationId, string thumbprint, string tenantId, string subscriptionId)
        {
            ClientId = clientId;
            Thumbprint = thumbprint;
            TenantId = tenantId;
            SubscriptionId = subscriptionId;
            ApplicationId = applicationId;
        }

        public string ClientId { get; }
        public string ApplicationId { get; }
        public string Thumbprint { get; }
        public string TenantId { get; }
        public string SubscriptionId { get; }
    }
}