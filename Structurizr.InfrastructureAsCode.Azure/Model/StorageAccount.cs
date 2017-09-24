using System;
using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class StorageAccount : ContainerInfrastructure, IHaveResourceId, IHttpsConnectionSource
    {
        public string EnvironmentInvariantName { get; set; }

        public StorageAccountKind Kind { get; set; }

        public StorageAccountAccessKey AccessKey => new StorageAccountAccessKey(this);

        public string ResourceIdReference =>
            $"[resourceId('Microsoft.Storage/storageAccounts', '{Name}')]";

        public IEnumerable<KeyValuePair<string, IConfigurationValue>> ConnectionInformation()
        {
            if (string.IsNullOrWhiteSpace(EnvironmentInvariantName))
            {
                throw new InvalidOperationException("You have to set the EnvironmentInvariantName in order to use this as a source of connections");
            }
            yield return new KeyValuePair<string, IConfigurationValue>(EnvironmentInvariantName + "-key", AccessKey);
            if (Kind == StorageAccountKind.BlobStorage)
            {
                yield return new KeyValuePair<string, IConfigurationValue>(EnvironmentInvariantName + "-endpoint-blob", new StorageAccountBlobEndpoint(this));
            }
        }
    }

    public enum StorageAccountKind
    {
        Storage,
        BlobStorage
    }

    public class StorageAccountBlobEndpoint : DependentConfigurationValue<StorageAccount>
    {
        public StorageAccountBlobEndpoint(StorageAccount dependentOn) : base(dependentOn)
        {
        }

        public override object Value => $"https://{DependsOn.Name}.blob.core.windows.net/";
        public override bool ShouldBeStoredSecure => false;
    }

    public class StorageAccountAccessKey : DependentConfigurationValue<StorageAccount>
    {
        public StorageAccountAccessKey(StorageAccount account)
            : base(account)
        {
        }

        public override bool ShouldBeStoredSecure => true;

        public override object Value =>
            $"[concat('DefaultEndpointsProtocol=https;AccountName=','{ DependsOn.Name}',';AccountKey=',listKeys(resourceId('Microsoft.Storage/storageAccounts', '{DependsOn.Name}'), '2015-05-01-preview').key1)]";
    }
}