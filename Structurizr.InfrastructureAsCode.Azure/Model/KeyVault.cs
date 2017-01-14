namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class KeyVault : ContainerInfrastructure
    {
        public KeyVault()
        {
            Secrets = new Configuration<KeyVaultSecret>();
        }

        public Configuration<KeyVaultSecret> Secrets { get; set; }

        public ConfigurationValue<string> Url => new ConfigurationValue<string>("TODO");

        public KeyVaultActiveDirectoryApplicationId ActiveDirectoryApplicationIdFor(string clientName)
        {
            return new KeyVaultActiveDirectoryApplicationId { ClientName = clientName };
        }

        public KeyVaultActiveDirectoryApplicationSecret ActiveDirectoryApplicationSecretFor(string clientName)
        {
            return new KeyVaultActiveDirectoryApplicationSecret { ClientName = clientName };
        }

        protected override bool IsNameValid(string name)
        {
            return base.IsNameValid(name) && name.Length >= 3 && name.Length <= 24;
        }
    }

    public class KeyVaultActiveDirectoryApplicationId : ConfigurationValue
    {
        public string ClientName { get; set; }
    }

    public class KeyVaultActiveDirectoryApplicationSecret : ConfigurationValue
    {
        public string ClientName { get; set; }
    }

    public class KeyVaultSecret : ConfigurationElement
    {
        public string Name { get; set; }
    }
}