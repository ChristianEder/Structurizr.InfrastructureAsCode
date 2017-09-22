using System;
using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class CosmosDocumentDatabase : ContainerInfrastructure, IHttpsConnectionSource
    {
        public FixedConfigurationValue<string> Uri => new FixedConfigurationValue<string>($"https://{Name}.documents.azure.com:443/");
        public CosmosDocumentDatabaseAccessKey PrimaryMasterKey => new CosmosDocumentDatabaseAccessKey(this)
        {
            Type = CosmosDatabaseAccessKeyType.Primary
        };

        public string EnvironmentInvariantName { get; set; }

        IEnumerable<KeyValuePair<string, IConfigurationValue>> IHttpsConnectionSource.ConnectionInformation()
        {
            if (string.IsNullOrWhiteSpace(EnvironmentInvariantName))
            {
                throw new InvalidOperationException("You have to set the EnvironmentInvariantName in order to use this as a source of connections");
            }
            yield return new KeyValuePair<string, IConfigurationValue>(EnvironmentInvariantName + "-url", Uri);
            yield return new KeyValuePair<string, IConfigurationValue>(EnvironmentInvariantName + "-key", PrimaryMasterKey);
        }

    }

    public class CosmosDocumentDatabaseAccessKey : ConfigurationValue
    {
        public CosmosDocumentDatabase Database { get; }

        public CosmosDocumentDatabaseAccessKey(CosmosDocumentDatabase database)
        {
            Database = database;
        }
        public CosmosDatabaseAccessKeyType Type { get; set; }

        
    }
    public enum CosmosDatabaseAccessKeyType
    {
        Primary
    }
}