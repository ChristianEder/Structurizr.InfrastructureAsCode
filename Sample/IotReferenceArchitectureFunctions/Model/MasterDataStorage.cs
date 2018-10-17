using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class MasterDataStorage : ContainerWithInfrastructure<SqlDatabase>
    {
        public MasterDataStorage(IotReferenceArchitectureWithFunctions iotReferenceArchitectureWithFunctions, SqlServer sqlServer, 
            IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchitectureWithFunctions.System.AddContainer(
                name: "Master Data Storage",
                description: "Stores master data",
                technology: "Azure SQL Database");

            Infrastructure = new SqlDatabase (sqlServer.Infrastructure)
            {
                Name = "masterdata" + environment.Name
            };
        }
    }
}
