using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class KeyVaultActiveDirectoryApplicationIdResolver : AzureContainerInfrastructureConfigurationElementValueResolver<KeyVaultActiveDirectoryApplicationId>
    {
        protected override Task<object> Resolve(KeyVaultActiveDirectoryApplicationId value)
        {
            return null;
        }
    }
}
