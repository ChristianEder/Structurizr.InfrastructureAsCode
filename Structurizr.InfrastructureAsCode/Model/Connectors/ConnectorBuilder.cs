using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public enum UsageDirection
    {
        ToUsedContainer,
        ToUsingContainer
    }

    public interface ICanConfigureConnectorDescription
    {
        void InOrderTo(string description);
        ICanConfigureConnectorDescription InvertUsage();
    }

    public interface ICanConfigureTechnologies<TUsing, TUsed> : ICanConfigureConnectorDescription
    {
        ICanConfigureTechnologies<TUsing, TUsed> Over(string technology);
        ICanConfigureConnectorDescription Over<TConnector>() where TConnector : IContainerConnector, new();
        ICanConfigureConnectorDescription Over<TConnector>(TConnector connector) where TConnector : IContainerConnector;
    }

    public class ConnectorBuilder<TUsing, TUsed> : ICanConfigureTechnologies<TUsing, TUsed>
         where TUsing : ContainerInfrastructure
         where TUsed : ContainerInfrastructure
    {
        private UsageDirection _direction = UsageDirection.ToUsedContainer;
        private readonly ContainerWithInfrastructure<TUsing> _usingContainer;
        private readonly ContainerWithInfrastructure<TUsed> _usedContainer;

        private readonly List<string> _connectorTechnologies = new List<string>();
        private IContainerConnector _containerConnector;

        public ConnectorBuilder(ContainerWithInfrastructure<TUsing> usingContainer, ContainerWithInfrastructure<TUsed> usedContainer)
        {
            _usingContainer = usingContainer;
            _usedContainer = usedContainer;
        }

        public ICanConfigureConnectorDescription Over<TConnector>()
            where TConnector : IContainerConnector, new()
        {
            return Over(new TConnector());
        }

        public ICanConfigureConnectorDescription Over<TConnector>(TConnector connector)
            where TConnector : IContainerConnector
        {
            _containerConnector = connector;
            _connectorTechnologies.Add(connector.Technology);
            return this;
        }

        public ICanConfigureTechnologies<TUsing, TUsed> Over(string technology)
        {
            _connectorTechnologies.Add(technology);
            return this;
        }

        public void InOrderTo(string description)
        {
            var technology = string.Join(" over ", _connectorTechnologies);

            ContainerWithInfrastructure usageTarget = _usedContainer;
            ContainerWithInfrastructure usageSource = _usingContainer;

            if (_direction == UsageDirection.ToUsingContainer)
            {
                usageTarget = _usingContainer;
                usageSource = _usedContainer;
            }

            if (!string.IsNullOrWhiteSpace(technology))
            {
                usageSource.Container.Uses(usageTarget.Container, description, technology);
            }
            else
            {
                usageSource.Container.Uses(usageTarget.Container, description);
            }

            var connector = _containerConnector
                ?? _usedContainer as IContainerConnector
                ?? _usedContainer.Infrastructure as IContainerConnector
                ?? _usingContainer as IContainerConnector
                ?? _usingContainer.Infrastructure as IContainerConnector;


            connector?.Connect(_usingContainer, _usedContainer);
        }

        public ICanConfigureConnectorDescription InvertUsage()
        {
            switch (_direction)
            {
                case UsageDirection.ToUsedContainer:
                    _direction = UsageDirection.ToUsingContainer;
                    break;
                case UsageDirection.ToUsingContainer:
                    _direction = UsageDirection.ToUsedContainer;
                    break;
            }
            return this;
        }
    }
}
