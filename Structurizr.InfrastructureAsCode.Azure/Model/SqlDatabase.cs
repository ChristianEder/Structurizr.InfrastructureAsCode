using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class SqlDatabase: ContainerInfrastructure, IHaveInfrastructure<SqlServer>
    { 
     
        public SqlDatabase(SqlServer sqlServer)
        {
            SqlServer = sqlServer;
            sqlServer.Databases.Add(this);
        }


        public SqlServer SqlServer { get; }
        SqlServer IHaveInfrastructure<SqlServer>.Infrastructure => SqlServer;
        ContainerInfrastructure IHaveInfrastructure.Infrastructure => SqlServer;
    }
}
