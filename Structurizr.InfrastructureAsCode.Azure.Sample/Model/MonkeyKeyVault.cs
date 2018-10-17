using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class MonkeyKeyVault : ContainerWithInfrastructure<KeyVault>
    {
        public MonkeyKeyVault(MonkeyFactory monkeyFactory, 
            IInfrastructureEnvironment environment)
        {
            Container = monkeyFactory.System.AddContainer(
                name: "Monkey Key Vault",
                description: "Keeps the secrets",
                technology: "HTTPS");

            Infrastructure = new KeyVault()
            {
                Name = "monkey-keyvault-" + environment.Name,
                EnvironmentInvariantName = "monkey-keyvault"
            };
        }
    }
}
