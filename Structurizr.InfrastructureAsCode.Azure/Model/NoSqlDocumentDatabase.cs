namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class NoSqlDocumentDatabase : ContainerInfrastructure
    {
        public ConfigurationValue<string> Uri => new ConfigurationValue<string>($"https://{Name}.documents.azure.com:443/");
        public NoSqlDocumentDatabaseAccessKey PrimaryMasterKey => new NoSqlDocumentDatabaseAccessKey { Type = "PrimaryMaster" };
    }

    public class NoSqlDocumentDatabaseAccessKey : ConfigurationValue
    {
        public string Type { get; set; }
    }
}