using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public interface IResourceLocationTargetingStrategy
    {
        string TargetLocation(IInfrastructureEnvironment environment, ContainerWithInfrastructure container);
    }
}