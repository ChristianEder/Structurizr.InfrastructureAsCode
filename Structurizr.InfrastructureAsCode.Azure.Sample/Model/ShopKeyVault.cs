using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopKeyVault : Container<KeyVault>
    {
        public ShopKeyVault() : this(null)
        {
            
        }

        public ShopKeyVault(IInfrastructureEnvironment environment)
        {
            Name = "Key Vault";
            Description = "Stores infrastructure secrets and allows applications access to those secrets at runtime";

            if (environment != null)
            {
                Infrastructure = new KeyVault {Name = $"aac-keyvault-{environment.Name}"};
            }
        }
    }
}