using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public class Amqp : ContainerConnector<IAmqpConnectionSource>
    {
        private readonly IAmqpConnectionSource _connectionSource;
        private readonly string _queue;

        public Amqp(IAmqpConnectionSource connectionSource, string queue)
        {
            _connectionSource = connectionSource;
            _queue = queue;
        }

        public override string Technology => "AMQP";

        public override void Connect<TUsing, TUsed>(ContainerWithInfrastructure<TUsing> usingContainer, ContainerWithInfrastructure<TUsed> usedContainer)
        {
            Configure(usingContainer, _connectionSource, failIfNoTarget: true);
            Configure(usedContainer, _connectionSource, failIfNoTarget: false);
        }

        protected override IEnumerable<KeyValuePair<string, ConfigurationValue>> ConnectionInformation(IAmqpConnectionSource source)
        {
            return source.ConnectionInformation(_queue);
        }
    }

    public interface IAmqpConnectionSource
    {
        IEnumerable<KeyValuePair<string, ConfigurationValue>> ConnectionInformation(string queue);
    }
}
