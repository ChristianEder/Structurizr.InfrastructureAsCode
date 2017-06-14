using System;

namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public abstract class ContainerConnector
    {
        public abstract string Technology { get; }

        public virtual void Connect<TUsing, TUsed>(ContainerWithInfrastructure<TUsing> usingContainer,
            ContainerWithInfrastructure<TUsed> usedContainer)
            where TUsing : ContainerInfrastructure
            where TUsed : ContainerInfrastructure
        {
            var connectionTarget = usingContainer.Infrastructure as IConnectionTarget ?? usingContainer as IConnectionTarget;
            if (connectionTarget == null)
            {
                throw new InvalidOperationException("When using connector classes, the using container has to have an infrastructure implementing the IConnectionTarget interface or implement IConnectionTarget itself. I did not figure out an easy way yet to check this at compile time...");
            }

            var connectionSource = usedContainer.Infrastructure as IConnectionSource ?? usedContainer as IConnectionSource;
            if (connectionSource == null)
            {
                throw new InvalidOperationException("When using connector classes, the used container has to have an infrastructure implementing the IConnectionSource interface or implement IConnectionSource itself. I did not figure out an easy way yet to check this at compile time...");
            }

            foreach (var c in connectionSource.GetConnectionInformation())
            {
                connectionTarget.Configure(c.Key, c.Value);
            }
        }
    }
}