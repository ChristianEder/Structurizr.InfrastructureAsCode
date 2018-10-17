using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class SqlServer : ContainerInfrastructure, IHaveResourceId
    {
        public string ApiVersion = "2015-05-01-preview";
        public IList<SqlDatabase> Databases { get; } = new List<SqlDatabase>();
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.Sql/servers', '{Name}')";

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";

        public string AdministratorLogin { get; set; }
        public string AdministratorPassword { get; set; }
    }
}