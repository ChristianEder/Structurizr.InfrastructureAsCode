using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class RestApi : ContainerWithInfrastructure<WebAppService>
    {
        public RestApi(IotReferenceArchitectureWithFunctions iotReferenceArchitectureWithFunctions, TelemetryStorage telemetryStorage, MasterDataStorage masterDataStorage, ApplicationInsights applicationInsights, IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchitectureWithFunctions.System.AddContainer(
                name: "REST Api",
                description: "Implements endpoint required by the UI to load data",
                technology: "Azure App Service");

            Infrastructure = new WebAppService
            {
                Name = "ref-api-" + environment.Name,
                EnvironmentInvariantName = "ref-api"
            };

            Uses(telemetryStorage).Over(telemetryStorage.Infrastructure.TableEndpoint).InOrderTo("Load telemetry data");
            Uses(masterDataStorage).InOrderTo("Load master data");
            Uses(applicationInsights).InOrderTo("Log metrics");
        }
    }
}