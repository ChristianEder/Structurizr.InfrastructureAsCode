using System;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class AppService : ContainerInfrastructure
    {
        public AppService(Func<IInfrastructureEnvironment, string> name) : base(name)
        {
        }
    }
}
