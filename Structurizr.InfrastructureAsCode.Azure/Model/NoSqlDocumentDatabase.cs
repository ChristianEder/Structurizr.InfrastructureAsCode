namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class NoSqlDocumentDatabase : ContainerInfrastructure
    {
        public NoSqlDocumentDatabaseAccessKey PrimaryMasterKey => new NoSqlDocumentDatabaseAccessKey { Type = "PrimaryMaster" };
    }

    public class NoSqlDocumentDatabaseAccessKey : ContainerInfrastructureConfigurationElementValue
    {
        public string Type { get; set; }
    }
}