using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public interface IAzureInfrastructureEnvironment : IInfrastructureEnvironment
    {
        string Tenant { get; }
        IEnumerable<string> AdministratorUserIds { get; }
    }
}
