namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class StorageAccount : ContainerInfrastructure, IHaveResourceId
    {
        public StorageAccountConnectionString ConnectionString => new StorageAccountConnectionString(this);

        public string ResourceIdReference =>
            $"[resourceId('Microsoft.Storage/storageAccounts', '{Name}')]";
    }

    public class StorageAccountConnectionString : IDependentConfigurationValue
    {
        private readonly StorageAccount _account;

        public StorageAccountConnectionString(StorageAccount account)
        {
            _account = account;
        }

        public object Value =>
            $"[concat('DefaultEndpointsProtocol=https;AccountName=','{_account.Name}',';AccountKey=',listKeys(resourceId('Microsoft.Storage/storageAccounts', '{_account.Name}'), '2015-05-01-preview').key1)]";

        public bool IsResolved => true;
        public IHaveResourceId DependsOn => _account;

        public override bool Equals(object obj)
        {
            return Equals(obj as StorageAccountConnectionString);
        }

        protected bool Equals(StorageAccountConnectionString other)
        {
            return !ReferenceEquals(null, other) && Equals(_account, other._account);
        }

        public override int GetHashCode()
        {
            return _account?.GetHashCode() ?? 0;
        }
    }
}