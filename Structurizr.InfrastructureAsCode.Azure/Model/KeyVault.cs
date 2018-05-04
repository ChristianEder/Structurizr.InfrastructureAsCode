using System;
using System.Collections.Generic;
using System.Linq;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class KeyVault : ContainerInfrastructure, ISecureConfigurationStore
    {

        public KeyVault()
        {
            Secrets = new Configuration<KeyVaultSecret>();
        }

        public string EnvironmentInvariantName { get; set; }

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.KeyVault/vaults', '{Name}')";
        public List<IHaveServiceIdentity> Readers { get; } = new List<IHaveServiceIdentity>();

        public Configuration<KeyVaultSecret> Secrets { get; set; }

        public IConfigurationValue Url => new FixedConfigurationValue<string>($"https://{Name}.vault.azure.net/");

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

        void ISecureConfigurationStore.Store(string name, IConfigurationValue value)
        {
            var existing = Secrets.FirstOrDefault(s => s.Name == name);
            if (!ReferenceEquals(existing, null))
            {
                if (!Equals(existing.Value, value))
                {
                    throw new InvalidOperationException();
                }
                return;
            }
            Secrets.Add(new KeyVaultSecret { Name = name, Value = value });
        }

        void ISecureConfigurationStore.AllowAccessFrom(IHaveServiceIdentity serviceIdentity)
        {
            if (Readers.Any(r => r.Id == serviceIdentity.Id))
            {
                return;
            }
            Readers.Add(serviceIdentity);
        }
    }

    public class KeyVaultActiveDirectoryApplicationId : ConfigurationValue
    {
        public string ClientName { get; set; }
        public override bool ShouldBeStoredSecure => false;
    }

    public class KeyVaultActiveDirectoryApplicationSecret : ConfigurationValue
    {
        public KeyVaultActiveDirectoryApplicationSecret(string clientName)
        {
            ClientName = clientName;
        }

        public string ClientName { get; }
        public override bool ShouldBeStoredSecure => true;

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