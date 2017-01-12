using System;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class KeyVault : ContainerInfrastructure
    {
        public KeyVault()
        {
            Secrets = new ContainerInfrastructureConfiguration<KeyVaultSecret>();
        }

        public ContainerInfrastructureConfiguration<KeyVaultSecret> Secrets { get; set; }

        public ContainerInfrastructureConfigurationElementValue<string> Url => new ContainerInfrastructureConfigurationElementValue<string>("TODO");

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

    public class KeyVaultActiveDirectoryApplicationId : ContainerInfrastructureConfigurationElementValue
    {
        public string ClientName { get; set; }
    }

    public class KeyVaultActiveDirectoryApplicationSecret : ContainerInfrastructureConfigurationElementValue
    {
        public string ClientName { get; set; }
    }

    public class KeyVaultSecret : ContainerInfrastructureConfigurationElement
    {
        public string Name { get; set; }
    }
}