using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopKeyVault : Container
    {
        public ShopKeyVault()
        {
            Name = "Key Vault";
            Description = "Stores infrastructure secrets and allows applications access to those secrets at runtime";
            Infrastructure = new KeyVault();
        }
    }
}