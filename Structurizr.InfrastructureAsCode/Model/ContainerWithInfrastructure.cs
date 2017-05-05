namespace Structurizr.InfrastructureAsCode
{
    public abstract class ContainerWithInfrastructure
    {
        public virtual void InitializeUsings()
        {
        }

        public Container Container { get; protected set; }
        public ContainerInfrastructure Infrastructure { get; protected set; }
    }

    public class ContainerWithInfrastructure<TInfrastructure> : ContainerWithInfrastructure where TInfrastructure : ContainerInfrastructure
    {
        public new TInfrastructure Infrastructure
        {
            get => (TInfrastructure)base.Infrastructure;
            protected set => base.Infrastructure = value;
        }
    }
}
