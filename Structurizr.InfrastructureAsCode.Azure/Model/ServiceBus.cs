using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class ServiceBus : ContainerInfrastructure
    {
        public ServiceBus()
        {
            Queues = new List<string>();
            Topics = new List<string>();
        }
        public List<string> Queues { get; }
        public List<string> Topics { get; }
    }
}