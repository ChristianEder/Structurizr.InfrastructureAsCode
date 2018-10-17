using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace IotReferenceArchitectureFunctions.Model
{
    public class UserInterface : ContainerWithInfrastructure<WebAppService>
    {
        public UserInterface(IotReferenceArchitectureWithFunctions iotReferenceArchitectureWithFunctions, RestApi restApi, IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchitectureWithFunctions.System.AddContainer(
                name: "User interface",
                description: "Visualizes the data, dashboarding etc.",
                technology: "Azure App Service");

            Infrastructure = new WebAppService
            {
                Name = "ref-ui-" + environment.Name,
                EnvironmentInvariantName = "ref-ui"
            };

            Uses(restApi).Over<Https>().InOrderTo("Load data");
        }
    }
}