using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class SecretStorage : ContainerWithInfrastructure<KeyVault>
    {
        public SecretStorage(IotReferenceArchitectureWithFunctions iotReferenceArchitectureWithFunctions,
            IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchitectureWithFunctions.System.AddContainer(
                "Secret storage",
                "Stores secrets that other services require to access each other",
                "Azure Key Vault");

            Infrastructure = new KeyVault
            {
                Name = "ref-keyvault-" + environment.Name
            };
        }
    }
}