using System;
using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public abstract class ContainerConnector
    {
        public abstract string Technology { get; }

        public abstract void Connect<TUsing, TUsed>(ContainerWithInfrastructure<TUsing> usingContainer,
            ContainerWithInfrastructure<TUsed> usedContainer)
            where TUsing : ContainerInfrastructure
            where TUsed : ContainerInfrastructure;
    }


    public abstract class ContainerConnector<TConnectionSource> : ContainerConnector
        where TConnectionSource : class
    {

        public override void Connect<TUsing, TUsed>(
            ContainerWithInfrastructure<TUsing> usingContainer,
            ContainerWithInfrastructure<TUsed> usedContainer)
        {
            var connectionSource = usedContainer.Infrastructure as TConnectionSource ?? usedContainer as TConnectionSource;
            if (connectionSource == null)
            {
                throw new InvalidOperationException($"When using connector classes, the used container has to have an infrastructure implementing the {typeof(TConnectionSource).Name} interface or implement it itself. I did not figure out an easy way yet to check this at compile time...");
            }

            Configure(usingContainer, connectionSource);
        }

        protected void Configure(ContainerWithInfrastructure target, TConnectionSource connectionSource, bool failIfNoTarget = true)
        {
            var connectionTarget = GetConfigurable(target, failIfNoTarget);

            if (!ReferenceEquals(null, connectionTarget))
            {
                foreach (var c in ConnectionInformation(connectionSource))
                {
                    connectionTarget.Configure(c.Key, c.Value);
                }
            }
        }

        private IConfigurable GetConfigurable(ContainerWithInfrastructure container, bool failIfNoTarget)
        {
            var configurable = container.Infrastructure as IConfigurable ?? container as IConfigurable;
            if (configurable == null && failIfNoTarget)
            {
                throw new InvalidOperationException(
                    "When using connector classes, the using container has to have an infrastructure implementing the IConfigurable interface or implement IConfigurable itself. I did not figure out an easy way yet to check this at compile time...");
            }
            return configurable;
        }

        protected abstract IEnumerable<KeyValuePair<string, ConfigurationValue>> ConnectionInformation(TConnectionSource source);
    }
}