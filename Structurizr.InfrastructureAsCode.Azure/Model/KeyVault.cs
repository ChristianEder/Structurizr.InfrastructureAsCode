using System;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class KeyVault : ContainerInfrastructure
    {
        public KeyVault(Func<IInfrastructureEnvironment, string> name) : base(name)
        {
        }
    }
}