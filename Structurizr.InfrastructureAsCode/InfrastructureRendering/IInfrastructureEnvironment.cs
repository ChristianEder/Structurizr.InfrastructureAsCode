using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering
{
    public interface IInfrastructureEnvironment
    {
        string Name { get; }
        string Tenant { get; }
        IEnumerable<string> AdministratorUserIds { get; }
    }
}