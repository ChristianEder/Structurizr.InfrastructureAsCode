using System;
using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class NoSqlDocumentDatabase : ContainerInfrastructure, IConnectionSource
    {
        public ConfigurationValue<string> Uri => new ConfigurationValue<string>($"https://{Name}.documents.azure.com:443/");
        public NoSqlDocumentDatabaseAccessKey PrimaryMasterKey => new NoSqlDocumentDatabaseAccessKey { Type = "PrimaryMaster" };

        public string EnvironmentInvariantName { get; set; }

        IEnumerable<KeyValuePair<string, ConfigurationValue>> IConnectionSource.GetConnectionInformation()
        {
            if (string.IsNullOrWhiteSpace(EnvironmentInvariantName))
            {
                throw new InvalidOperationException("You have to set the EnvironmentInvariantName in order to use this as a source of connections");
            }
            yield return new KeyValuePair<string, ConfigurationValue>(EnvironmentInvariantName + "-url", Uri);
            yield return new KeyValuePair<string, ConfigurationValue>(EnvironmentInvariantName + "-key", PrimaryMasterKey);
        }
    }

    public class NoSqlDocumentDatabaseAccessKey : ConfigurationValue
    {
        public string Type { get; set; }
    }
}