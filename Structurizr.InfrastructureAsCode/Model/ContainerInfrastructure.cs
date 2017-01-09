using System;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode
{
    public class ContainerInfrastructure
    {
        public ContainerInfrastructure(Func<IInfrastructureEnvironment, string> name)
        {
            Name = name;
        }

        public Func<IInfrastructureEnvironment, string> Name { get; }
    }
}