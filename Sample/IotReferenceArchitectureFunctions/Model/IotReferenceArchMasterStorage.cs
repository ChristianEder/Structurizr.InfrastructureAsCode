using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class IotReferenceArchMasterStorage : ContainerWithInfrastructure<SqlDatabase>
    {
        public IotReferenceArchMasterStorage(IotReferenceArchModel iotReferenceArchModel, IotReferenceArchSqlServer iotReferencecArchSqlServer, 
            IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchModel.System.AddContainer(
                name: "Master SQL Storage",
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
