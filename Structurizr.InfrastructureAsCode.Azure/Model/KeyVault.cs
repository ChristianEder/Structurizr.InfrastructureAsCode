using System;
using System.Linq;
using Microsoft.Azure.Management.AppService.Fluent;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class KeyVault : ContainerInfrastructure, IConfigurable
    {
        public KeyVault()
        {
            Secrets = new Configuration<KeyVaultSecret>();
        }

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.KeyVault/vaults', '{Name}')";

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

        void IConfigurable.Configure(string name, IConfigurationValue value)
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

        bool IConfigurable.IsConfigurationDependentOn(IHaveInfrastructure other)
        {
            var resource = other.Infrastructure as IHaveResourceId;
            if (resource == null)
            {
                return false;
            }

            return Secrets
                .Select(s => s.Value)
                .OfType<IDependentConfigurationValue>()
                .Any(v => v.DependsOn == resource);
        }

        void IConfigurable.UseStore(IConfigurable store)
        {
            throw new InvalidOperationException();
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