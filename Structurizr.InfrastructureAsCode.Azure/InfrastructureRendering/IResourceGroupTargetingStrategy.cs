using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public interface IResourceGroupTargetingStrategy
    {
        string TargetResourceGroup(IInfrastructureEnvironment environment, ContainerWithInfrastructure container);
    }
}