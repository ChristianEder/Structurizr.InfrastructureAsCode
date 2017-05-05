using System;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class ResourceGroupPerEnvironmentStrategy : IResourceGroupTargetingStrategy
    {
        private readonly Func<IInfrastructureEnvironment, string> _namingConvention;

        public ResourceGroupPerEnvironmentStrategy(Func<IInfrastructureEnvironment, string> namingConvention)
        {
            _namingConvention = namingConvention;
        }

        public string TargetResourceGroup(IInfrastructureEnvironment environment, ContainerWithInfrastructure container)
        {
            return _namingConvention(environment);
        }
    }
}