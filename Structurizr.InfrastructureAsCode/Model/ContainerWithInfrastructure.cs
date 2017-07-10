using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode
{
    public abstract class ContainerWithInfrastructure : IHaveInfrastructure
    {
        public virtual void InitializeUsings()
        {
        }

        public Container Container { get; protected set; }
        public ContainerInfrastructure Infrastructure { get; protected set; }
    }

    public class ContainerWithInfrastructure<TInfrastructure> : ContainerWithInfrastructure, IHaveInfrastructure<TInfrastructure>
        where TInfrastructure : ContainerInfrastructure
    {
        public new TInfrastructure Infrastructure
        {
            get => (TInfrastructure)base.Infrastructure;
            protected set => base.Infrastructure = value;
        }

        public ICanConfigureTechnologies<TInfrastructure, TOtherInfrastructure> Uses<TOtherInfrastructure>(ContainerWithInfrastructure<TOtherInfrastructure> other)
            where TOtherInfrastructure : ContainerInfrastructure
        {
            return new ConnectorBuilder<TInfrastructure, TOtherInfrastructure>(this, other);
        }
    }
}
