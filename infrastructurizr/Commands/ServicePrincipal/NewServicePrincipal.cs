using System.Collections.Generic;

namespace infrastructurizr.Commands.ServicePrincipal
{
    public class NewServicePrincipal : Command
    {
        public override string Name => "new serviceprincipal";

        public override string Description =>
            "Creates a new service principal in your Azure AD along with a certificate for authentication, that is authorized to issue and manage resource group deployments.";
        public override IEnumerable<CommandParameter> Parameters { get; }

        public override void Execute()
        {
            
        }
    }
}
