using System;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class KeyVault : ContainerInfrastructure
    {
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