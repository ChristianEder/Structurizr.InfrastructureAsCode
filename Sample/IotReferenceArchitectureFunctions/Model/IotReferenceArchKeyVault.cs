using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class IotReferenceArchKeyVault : ContainerWithInfrastructure<KeyVault>
    {
        public IotReferenceArchKeyVault(IotReferenceArchModel iotReferenceArchModel,
            IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchModel.System.AddContainer(
                "Iot Reference Architecture Key Vault",
                "Keeps the secrets",
                "");

            Infrastructure = new KeyVault
            {
                Name = "ref-keyvault-" + environment.Name
            };
        }
    }
}