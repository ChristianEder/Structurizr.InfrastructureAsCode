using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class EventHubNamespace : ContainerInfrastructure, IHaveResourceId
    {
        public List<EventHub> Hubs { get; } = new List<EventHub>();
        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.EventHub/namespaces', '{Name}')";
        public string ApiVersion => "2017-04-01";
        public EventHubNamespaceSharedAccessPolicy RootManageSharedAccessPolicy => new EventHubNamespaceSharedAccessPolicy(this, "RootManageSharedAccessKey");

        public EventHubNamespaceConnectionString ConnectionString => new EventHubNamespaceConnectionString(this, RootManageSharedAccessPolicy);
    }

    public class EventHubNamespaceSharedAccessPolicy : DependentConfigurationValue<EventHubNamespace>
    {

        public EventHubNamespaceSharedAccessPolicy(EventHubNamespace eventHubNamespace, string name) : base(eventHubNamespace)
        {
            Name = name;
        }
        public string Name { get; }

        public override bool ShouldBeStoredSecure => true;
        public override object Value => $"[listKeys(resourceId('Microsoft.EventHub/namespaces/authorizationRules', '{DependsOn.Name}', '{Name}'), '{DependsOn.ApiVersion}').primaryKey]";

    }

    public class EventHubNamespaceConnectionString : DependentConfigurationValue<EventHubNamespace>
    {
        public EventHubNamespaceSharedAccessPolicy AccessPolicy { get; }

        public EventHubNamespaceConnectionString(EventHubNamespace eventHubNamespace, EventHubNamespaceSharedAccessPolicy accessPolicy) : base(eventHubNamespace)
        {
            AccessPolicy = accessPolicy;
        }

        public override bool ShouldBeStoredSecure => true;
        public override object Value => $"[listKeys(resourceId('Microsoft.EventHub/namespaces/authorizationRules', '{DependsOn.Name}', '{AccessPolicy.Name}'), '{DependsOn.ApiVersion}').primaryConnectionString]";

    }
}