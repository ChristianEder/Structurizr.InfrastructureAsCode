using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public interface ICanConfigureConnectorDescription
    {
        void InOrderTo(string description);
    }

    public interface ICanConfigureTechnologies<TUsing, TUsed> : ICanConfigureConnectorDescription
    {
        ICanConfigureTechnologies<TUsing, TUsed> Over(string technology);
        ICanConfigureConnectorDescription Over<TConnector>() where TConnector : ContainerConnector, new();
        ICanConfigureConnectorDescription Over<TConnector>(TConnector connector) where TConnector : ContainerConnector;
    }

   public class ConnectorBuilder<TUsing, TUsed> : ICanConfigureTechnologies<TUsing, TUsed>
        where TUsing : ContainerInfrastructure 
        where TUsed : ContainerInfrastructure
    {
        private readonly ContainerWithInfrastructure<TUsing> _usingContainer;
        private readonly ContainerWithInfrastructure<TUsed> _usedContainer;

        private readonly List<string> _connectorTechnologies = new List<string>();
        private ContainerConnector _containerConnector;

        public ConnectorBuilder(ContainerWithInfrastructure<TUsing> usingContainer, ContainerWithInfrastructure<TUsed> usedContainer)
        {
            _usingContainer = usingContainer;
            _usedContainer = usedContainer;
        }

        public ICanConfigureConnectorDescription Over<TConnector>()
            where TConnector : ContainerConnector, new()
        {
            return Over(new TConnector());
        }

        public ICanConfigureConnectorDescription Over<TConnector>(TConnector connector)
            where TConnector : ContainerConnector
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
            if (!string.IsNullOrWhiteSpace(technology))
            {
                _usingContainer.Container.Uses(_usedContainer.Container, description, technology);
            }
            else
            {
                _usingContainer.Container.Uses(_usedContainer.Container, description);
            }

            _containerConnector?.Connect(_usingContainer, _usedContainer);
        }
    }
}
