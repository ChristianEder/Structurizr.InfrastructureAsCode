namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public interface IAzureSubscriptionCredentials
    {
        string ClientId { get; }
        string ClientSecret { get; }
        string TenantId { get; }
        string SubscriptionId { get; }
    }
}