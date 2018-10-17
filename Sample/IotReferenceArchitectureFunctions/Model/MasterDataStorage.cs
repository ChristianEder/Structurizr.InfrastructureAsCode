using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class MasterDataStorage : ContainerWithInfrastructure<SqlDatabase>
    {
        public MasterDataStorage(IotReferenceArchitectureWithFunctions iotReferenceArchitectureWithFunctions, SqlServer iotReferencecArchSqlServer, 
            IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchitectureWithFunctions.System.AddContainer(
                name: "Master Data Storage",
                description: "Stores master data",
                technology: "Azure SQL Database");

            var sqlServer = iotReferencecArchSqlServer.Infrastructure;

            Infrastructure = new SqlDatabase (sqlServer)
            {
                Name = "masterdata" + environment.Name
            };
        }
    }
}
