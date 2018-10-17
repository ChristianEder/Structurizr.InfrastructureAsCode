using System;
using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class EventHub : ContainerInfrastructure, IHaveInfrastructure<EventHubNamespace>
    {
        public EventHub(EventHubNamespace eventHubNamespace)
        {
            Namespace = eventHubNamespace;
            eventHubNamespace.Hubs.Add(this);
        }
        public string EnvironmentInvariantName { get; set; }
        public EventHubNamespace Namespace { get; }
        EventHubNamespace IHaveInfrastructure<EventHubNamespace>.Infrastructure => Namespace;
        ContainerInfrastructure IHaveInfrastructure.Infrastructure => Namespace;
    }

    public class EventHubSDK : ContainerConnector<EventHub>
    {
        public override string Technology => "Event Hub SDK";

        protected override IEnumerable<KeyValuePair<string, IConfigurationValue>> ConnectionInformation(EventHub source)
        {
            if (string.IsNullOrWhiteSpace(source.EnvironmentInvariantName))
            {
                throw new InvalidOperationException("You have to set the EnvironmentInvariantName in order to use this as a source of connections");
            }

            yield return new KeyValuePair<string, IConfigurationValue>(source.EnvironmentInvariantName + "-connection", source.Namespace.ConnectionString);
        }
    }
}