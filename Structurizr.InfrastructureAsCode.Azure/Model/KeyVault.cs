namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class KeyVault : ContainerInfrastructure
    {
        public KeyVault()
        {
            Secrets = new Configuration<KeyVaultSecret>();
        }

        public Configuration<KeyVaultSecret> Secrets { get; set; }

        public FixedConfigurationValue<string> Url => new FixedConfigurationValue<string>($"https://{Name}.vault.azure.net/");

        public KeyVaultActiveDirectoryApplicationId ActiveDirectoryApplicationIdFor(string clientName)
        {
            return new KeyVaultActiveDirectoryApplicationId { ClientName = clientName };
        }

        public KeyVaultActiveDirectoryApplicationSecret ActiveDirectoryApplicationSecretFor(string clientName)
        {
            return new KeyVaultActiveDirectoryApplicationSecret(clientName);
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
        public KeyVaultActiveDirectoryApplicationSecret(string clientName)
        {
            ClientName = clientName;
        }

        public string ClientName { get; }

        public override bool Equals(object obj)
        {
            var other = obj as KeyVaultActiveDirectoryApplicationSecret;
            return other != null && Equals(other);
        }

        protected bool Equals(KeyVaultActiveDirectoryApplicationSecret other)
        {
            return string.Equals(ClientName, other.ClientName);
        }

        public override int GetHashCode()
        {
            return ClientName?.GetHashCode() ?? 0;
        }
    }

    public class KeyVaultSecret : ConfigurationElement
    {
    }
}