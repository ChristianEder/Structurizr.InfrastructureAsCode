namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public interface IConfigurable
    {
        void Configure(string name, IConfigurationValue value);
        bool IsConfigurationDependentOn(IHaveInfrastructure other);
    }
}