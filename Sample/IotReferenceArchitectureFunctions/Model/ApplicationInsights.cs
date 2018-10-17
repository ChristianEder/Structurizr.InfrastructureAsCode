using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class ApplicationInsights : ContainerWithInfrastructure<Structurizr.InfrastructureAsCode.Azure.Model.ApplicationInsights>
    {

        public ApplicationInsights(IotReferenceArchitectureWithFunctions iotReferenceArchitectureWithFunctions, 
            IInfrastructureEnvironment environment)
        {

            Container = iotReferenceArchitectureWithFunctions.System.AddContainer(
                name: "Application Insights",
                description: "Serves as a logging target and monitoring tool",
                technology: "Azure Application Insights");

            Infrastructure = new Structurizr.InfrastructureAsCode.Azure.Model.ApplicationInsights
            {
                Name = "ref-ai-" + environment.Name
            };
        }


    }
}
