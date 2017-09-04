namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public interface IDependentConfigurationValue : IConfigurationValue
    {
        IHaveResourceId DependsOn { get; }
    }
}