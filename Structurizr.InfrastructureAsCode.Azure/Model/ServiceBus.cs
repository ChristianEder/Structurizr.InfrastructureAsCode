using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class ServiceBus : ContainerInfrastructure, IAmqpConnectionSource
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

        public IEnumerable<KeyValuePair<string, ConfigurationValue>> ConnectionInformation(string queue)
        {
            yield return new KeyValuePair<string, ConfigurationValue>(EnvironmentInvariantName + "-connectionstring", ConnectionString);
        }
    }

    public class ServiceBusConnectionString : ConfigurationValue
    {
        public ServiceBus ServiceBus { get; }

        public ServiceBusConnectionString(ServiceBus serviceBus)
        {
            ServiceBus = serviceBus;
        }
    }
}