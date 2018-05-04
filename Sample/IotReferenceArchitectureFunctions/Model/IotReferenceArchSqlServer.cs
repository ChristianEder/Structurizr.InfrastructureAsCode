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
    public class IotReferenceArchSqlServer: IHaveInfrastructure<SqlServer>
    {
   

        public IotReferenceArchSqlServer(IotReferenceArchModel iotReferenceArchModel,
            IInfrastructureEnvironment environment)
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
