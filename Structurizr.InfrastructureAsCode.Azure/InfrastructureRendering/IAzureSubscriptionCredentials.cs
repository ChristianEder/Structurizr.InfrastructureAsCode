namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public interface IAzureSubscriptionCredentials
    {
        string ClientId { get; }
        string ApplicationId { get; }
        string Thumbprint { get; }
        string TenantId { get; }
        string SubscriptionId { get; }
    }
}