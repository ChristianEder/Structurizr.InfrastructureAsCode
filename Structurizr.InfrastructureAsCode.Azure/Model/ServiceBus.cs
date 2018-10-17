using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class ServiceBus : ContainerInfrastructure, IAmqpConnectionSource, IHaveResourceId
    {
        public ServiceBus()
        {
            Queues = new List<string>();
            Topics = new List<string>();
        }

        public string EnvironmentInvariantName { get; set; }
        public ServiceBusConnectionString ConnectionString  => new ServiceBusConnectionString(this);

        public List<string> Queues { get; }
        public List<string> Topics { get; }

        public Amqp Queue(string queue)
        {
            if (!Queues.Contains(queue))
            {
                Queues.Add(queue);
            }
            return new Amqp(this, queue);
        }

        public IEnumerable<KeyValuePair<string, IConfigurationValue>> ConnectionInformation(string queue)
        {
            yield return new KeyValuePair<string, IConfigurationValue>(EnvironmentInvariantName + "-connectionstring", ConnectionString);
        }
        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.ServiceBus/namespaces', '{Name}')";
    }

    public class ServiceBusConnectionString : DependentConfigurationValue<ServiceBus>
    {
        public ServiceBusConnectionString(ServiceBus serviceBus) : base(serviceBus)
        {
        }

        public override bool ShouldBeStoredSecure => true;

        public override object Value => $"[listKeys(resourceId('Microsoft.ServiceBus/namespaces/authorizationRules', '{DependsOn.Name}', 'RootManageSharedAccessKey'), '2014-09-01').primaryConnectionString]";

    }
}