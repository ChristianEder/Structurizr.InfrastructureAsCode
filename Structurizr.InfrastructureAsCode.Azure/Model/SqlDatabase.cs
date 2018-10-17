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
