using System;
using System.Collections.Generic;
using Microsoft.Azure.Management.CosmosDB.Fluent;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class CosmosDocumentDatabase : ContainerInfrastructure, IHttpsConnectionSource, IHaveResourceId
    {
        public FixedConfigurationValue<string> Uri => new FixedConfigurationValue<string>($"https://{Name}.documents.azure.com:443/");
        public CosmosDocumentDatabaseAccessKey PrimaryMasterKey => new CosmosDocumentDatabaseAccessKey(this)
        {
            Type = CosmosDatabaseAccessKeyType.Primary
        };

        public string EnvironmentInvariantName { get; set; }

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.DocumentDb/databaseAccounts', '{Name}')";

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

    public class CosmosDocumentDatabaseAccessKey : DependentConfigurationValue<CosmosDocumentDatabase>
    {
        public CosmosDocumentDatabaseAccessKey(CosmosDocumentDatabase database) : base(database)
        {
        }

        public CosmosDatabaseAccessKeyType Type { get; set; }
        public override bool ShouldBeStoredSecure => true;
        public override object Value => $"[listKeys(resourceId('Microsoft.DocumentDb/databaseAccounts', '{DependsOn.Name}'), '2015-04-08').{KeyTypeIdentifier}]";

        private string KeyTypeIdentifier => Type == CosmosDatabaseAccessKeyType.Primary
            ? "primaryMasterKey"
            : throw new InvalidOperationException("Currently, only primaryMasterKey is supported");
    }
    public enum CosmosDatabaseAccessKeyType
    {
        Primary
    }
}