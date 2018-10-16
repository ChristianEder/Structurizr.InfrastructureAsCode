using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class IotReferenceArchSqlServer: IHaveInfrastructure<SqlServer>
    {
        public IotReferenceArchSqlServer(IInfrastructureEnvironment environment)
        {
            Infrastructure = new SqlServer
            {
                Name = "ref-sql-server-" + environment.Name

            };
        }

        ContainerInfrastructure IHaveInfrastructure.Infrastructure => Infrastructure;

        public SqlServer Infrastructure { get; }
    }
}
