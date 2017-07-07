using System;
using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class NoSqlDocumentDatabaseAccessKeyResolver : ConfigurationValueResolver<NoSqlDocumentDatabaseAccessKey>
    {
        private readonly AzureConfigurationValueResolverContext _context;

        public NoSqlDocumentDatabaseAccessKeyResolver(AzureConfigurationValueResolverContext context)
        {
            _context = context;
        }

        public override bool CanResolve(NoSqlDocumentDatabaseAccessKey value)
        {
            switch (value.Type)
            {
                case NoSqlDocumentDatabaseAccessKeyType.Primary:
                    return true;
                default:
                    return false;
            }
        }

        public override async Task<object> Resolve(NoSqlDocumentDatabaseAccessKey value)
        {

            var account =
                await _context.Azure.DocumentDBAccounts.GetByResourceGroupAsync(_context.ResourceGroupName,
                    value.Database.Name);
            var keys = await account.ListKeysAsync();

            switch (value.Type)
            {
                case NoSqlDocumentDatabaseAccessKeyType.Primary:
                    return keys.PrimaryMasterKey;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}