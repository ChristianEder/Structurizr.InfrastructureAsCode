namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public interface IConnectionTarget
    {
        void Configure(string name, ConfigurationValue value);
    }
}