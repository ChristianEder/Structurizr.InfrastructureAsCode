using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering
{
    public class InfrastructureEnvironment : IInfrastructureEnvironment {
        public InfrastructureEnvironment(string name, string tenant, IEnumerable<string> administratorUserIds)
        {
            Name = name;
            Tenant = tenant;
            AdministratorUserIds = administratorUserIds;
        }

        public string Name { get; }
        public string Tenant { get; }
        public IEnumerable<string> AdministratorUserIds { get; }
    }
}