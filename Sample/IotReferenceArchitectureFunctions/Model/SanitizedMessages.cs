using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class SanitizedMessages : ContainerWithInfrastructure<EventHub>
    {
        public SanitizedMessages(IotReferenceArchitectureWithFunctions iotReferenceArchitectureWithFunctions, EventHubNamespace eventHubNamespace, IInfrastructureEnvironment environment)
        {
            Container = iotReferenceArchitectureWithFunctions.System.AddContainer(
                name: "Sanitized messages",
                description: "Contains messages from devices that have been deserialized, sanitized and checked for security properties",
                technology: "Azure Event Hub");

            Infrastructure = new EventHub(eventHubNamespace.Infrastructure)
            {
                Name = "sanitized-messages",
                EnvironmentInvariantName = "sanitized-messages"
            };
        }
    }
}