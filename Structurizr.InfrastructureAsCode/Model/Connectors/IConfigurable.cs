
namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public interface IConfigurable
    {
        void Configure(string name, IConfigurationValue value);
        bool IsConfigurationDependentOn(IHaveInfrastructure other);
        void UseStore(ISecureConfigurationStore store);
    }

    public interface ISecureConfigurationStore
    {
        string EnvironmentInvariantName { get; }
        IConfigurationValue Url { get; }
        void Store(string name, IConfigurationValue value);
        void AllowAccessFrom(IHaveServiceIdentity serviceIdentity);
    }


    public interface IHaveServiceIdentity
    {
        string Id { get; }
    }
}