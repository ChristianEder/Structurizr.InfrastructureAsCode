using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent.Models;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;
using Structurizr.InfrastructureAsCode.Policies;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class KeyVaultActiveDirectoryApplicationIdResolver :
        ConfigurationValueResolver<KeyVaultActiveDirectoryApplicationId>,
        IConfigurationValueResolver<KeyVaultActiveDirectoryApplicationSecret>
    {
        private readonly AzureConfigurationValueResolverContext _context;
        private readonly IPasswordPolicy _passwordPolicy;

        public KeyVaultActiveDirectoryApplicationIdResolver(AzureConfigurationValueResolverContext context,
            IPasswordPolicy passwordPolicy)
        {
            _context = context;
            _passwordPolicy = passwordPolicy;
        }

        public override bool CanResolve(KeyVaultActiveDirectoryApplicationId value)
        {
            var application = _context.Graph.Applications.ListAsync().Result.SingleOrDefault(a => a.DisplayName == value.ClientName);
            return application == null;
        }

        public override async Task<object> Resolve(KeyVaultActiveDirectoryApplicationId value)
        {
            var password = _passwordPolicy.GetPassword();

            var applicationCreateParametersInner = new ApplicationCreateParametersInner
            {
                DisplayName = value.ClientName,
                Homepage = $"https://{value.ClientName}",
                IdentifierUris = new List<string> {$"https://{value.ClientName}"},
                PasswordCredentials =
                    new List<PasswordCredential>
                    {
                        new PasswordCredential { EndDate = DateTime.Now.AddYears(1000), Value = password}
                    }
            };

            var application = await _context.Graph.Applications.CreateAsync(applicationCreateParametersInner);

            _context.Values.Add(new KeyVaultActiveDirectoryApplicationSecret(value.ClientName), password);

            return application.AppId;
        }

        public Task<object> Resolve(KeyVaultActiveDirectoryApplicationSecret value)
        {
            return Task.FromResult(_context.Values[value]);
        }

        public bool CanResolve(KeyVaultActiveDirectoryApplicationSecret value)
        {
            return _context.Values.ContainsKey(value);
        }
    }
}
