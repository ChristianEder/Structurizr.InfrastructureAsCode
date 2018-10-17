using System;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class StorageAccount : ContainerInfrastructure, IHaveResourceId
    {
        public string EnvironmentInvariantName { get; set; }

        public StorageAccountKind Kind { get; set; }

        public StorageAccountConnectionString ConnectionString => new StorageAccountConnectionString(this);
        public StorageAccountBlobEndpoint BlobEndpoint => new StorageAccountBlobEndpoint(this);
        public StorageAccountTableEndpoint TableEndpoint => new StorageAccountTableEndpoint(this);
        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.Storage/storageAccounts', '{Name}')";

        public string AccountKey => $"[{AccountKeyValue}]";
        public string AccountKeyValue => $"listKeys(resourceId('Microsoft.Storage/storageAccounts', '{Name}'), '2015-05-01-preview').key1";
    }

    public enum StorageAccountKind
    {
        StorageV2
    }

    public abstract class StorageAccountEndpoint : IContainerConnector
    {
        private readonly StorageAccount _storageAccount;

        protected StorageAccountEndpoint(StorageAccount storageAccount)
        {
            _storageAccount = storageAccount;
        }

        public void Connect<TUsing, TUsed>(
            ContainerWithInfrastructure<TUsing> usingContainer, 
            ContainerWithInfrastructure<TUsed> usedContainer) 
            where TUsing : ContainerInfrastructure 
            where TUsed : ContainerInfrastructure
        {
            if (!ReferenceEquals(usedContainer.Infrastructure, _storageAccount))
            {
                throw new InvalidOperationException();
            }

            var configurable = ContainerConnector.GetConfigurable(usingContainer);
            configurable.Configure($"{_storageAccount.EnvironmentInvariantName}-connection-string", _storageAccount.ConnectionString);
        }

        public abstract string Technology { get; }
    }

    public class StorageAccountBlobEndpoint : StorageAccountEndpoint
    {
        public StorageAccountBlobEndpoint(StorageAccount storageAccount) : base(storageAccount)
        {
        }

        public override string Technology => "Blob SDK";
    }

    public class StorageAccountTableEndpoint : StorageAccountEndpoint
    {
        public StorageAccountTableEndpoint(StorageAccount storageAccount) : base(storageAccount)
        {
        }

        public override string Technology => "Table SDK";
    }

    public class StorageAccountConnectionString : DependentConfigurationValue<StorageAccount>
    {
        public StorageAccountConnectionString(StorageAccount account)
            : base(account)
        {
        }

        public override bool ShouldBeStoredSecure => true;

        public override object Value =>
            $"[concat('DefaultEndpointsProtocol=https;AccountName=','{ DependsOn.Name}',';AccountKey=',{DependsOn.AccountKeyValue})]";
    }
}