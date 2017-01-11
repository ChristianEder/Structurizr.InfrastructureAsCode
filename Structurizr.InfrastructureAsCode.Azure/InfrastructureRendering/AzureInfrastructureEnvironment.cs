using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class AzureInfrastructureEnvironment : InfrastructureEnvironment, IAzureInfrastructureEnvironment
    {
        public AzureInfrastructureEnvironment(string name, string tenant, IEnumerable<string> administratorUserIds)
            : base(name)
        {
            Tenant = tenant;
            AdministratorUserIds = administratorUserIds;
        }

        public string Tenant { get; }
        public IEnumerable<string> AdministratorUserIds { get; }
    }
}