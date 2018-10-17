using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class EventHubNamespace : IHaveInfrastructure<Structurizr.InfrastructureAsCode.Azure.Model.EventHubNamespace>
    {
        public EventHubNamespace(IInfrastructureEnvironment environment)
        {
            Infrastructure = new Structurizr.InfrastructureAsCode.Azure.Model.EventHubNamespace
            {
                Name = "ref-events-" + environment.Name
            };
        }

        ContainerInfrastructure IHaveInfrastructure.Infrastructure => Infrastructure;

        public Structurizr.InfrastructureAsCode.Azure.Model.EventHubNamespace Infrastructure { get; }
    }
}