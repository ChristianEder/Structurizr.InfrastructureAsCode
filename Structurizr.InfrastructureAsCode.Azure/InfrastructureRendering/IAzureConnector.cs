using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Structurizr.InfrastructureAsCode.IoC;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public interface IAzureConnector
    {
        IAzure Azure();

        IGraphRbacManagementClient Graph();
    }
}