using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class KeyVaultActiveDirectoryApplicationIdResolver : ConfigurationValueResolver<KeyVaultActiveDirectoryApplicationId>
    {
        private readonly AzureConfigurationValueResolverContext _context;

        public KeyVaultActiveDirectoryApplicationIdResolver(AzureConfigurationValueResolverContext context)
        {
            _context = context;
        }

        public override bool CanResolve(KeyVaultActiveDirectoryApplicationId value)
        {
            return false;
        }

        public override Task<object> Resolve(KeyVaultActiveDirectoryApplicationId value)
        {
            return null;
        }
    }
}
