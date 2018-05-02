using System;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class FunctionAppService : AppService, IHaveInfrastructure<StorageAccount>
    {
        private StorageAccount _storageAccount;

        public FunctionAppService()
        {
            Settings.Add(new AppServiceSetting("FUNCTIONS_EXTENSION_VERSION", "~1"));
            Settings.Add(new AppServiceSetting("WEBSITE_NODE_DEFAULT_VERSION", "6.5.0"));
        }

        protected override void OnNameChanged(string oldName, string newName)
        {
            base.OnNameChanged(oldName, newName);
            if (_storageAccount != null)
            {
                throw new InvalidOperationException("You cannot change this function apps name, once its associated storage account has been created.");
            }
        }

        public StorageAccount StorageAccount
        {
            get
            {
                _storageAccount = _storageAccount ?? DefineStorageAccount();
                return _storageAccount;
            }
        }

        private StorageAccount DefineStorageAccount()
        {
            var nameParts = Name.Split(new[] {'-', '_'}, StringSplitOptions.RemoveEmptyEntries);

            string name = "";
            foreach (var namePart in nameParts)
            {
                var shortPart = namePart.Substring(0, Math.Min(6, namePart.Length));
                var charactersLeft = 19 - name.Length;
                if (shortPart.Length <= charactersLeft || charactersLeft >= 3)
                {
                    shortPart = shortPart.Substring(0, Math.Min(shortPart.Length, charactersLeft));
                    name += shortPart;
                }
            }
            name += "store";

            var storage = new StorageAccount
            {

                Name = name
            };

            Settings.Add(new AppServiceSetting("AzureWebJobsDashboard", storage.ConnectionString));
            Settings.Add(new AppServiceSetting("AzureWebJobsStorage", storage.ConnectionString));
            Settings.Add(new AppServiceSetting("WEBSITE_CONTENTAZUREFILECONNECTIONSTRING", storage.ConnectionString));
            Settings.Add(new AppServiceSetting("WEBSITE_CONTENTSHARE", Name.ToLowerInvariant() + "-storage"));

            return storage;
        }

        ContainerInfrastructure IHaveInfrastructure.Infrastructure => StorageAccount;

        StorageAccount IHaveInfrastructure<StorageAccount>.Infrastructure => StorageAccount;
    }
}