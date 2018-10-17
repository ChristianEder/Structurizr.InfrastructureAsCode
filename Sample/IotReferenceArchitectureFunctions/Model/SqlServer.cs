using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class SqlServer: IHaveInfrastructure<Structurizr.InfrastructureAsCode.Azure.Model.SqlServer>
    {
        public SqlServer(IInfrastructureEnvironment environment)
        {
            Infrastructure = new Structurizr.InfrastructureAsCode.Azure.Model.SqlServer
            {
                Name = "ref-sql-server-" + environment.Name,
                AdministratorLogin = "adminsUserName",
                AdministratorPassword = "SomePass:word!"
            };
        }

        ContainerInfrastructure IHaveInfrastructure.Infrastructure => Infrastructure;

        public Structurizr.InfrastructureAsCode.Azure.Model.SqlServer Infrastructure { get; }
    }
}
