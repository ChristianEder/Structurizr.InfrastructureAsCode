namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public interface IConfigurable
    {
        void Configure(string name, ConfigurationValue value);
    }
}