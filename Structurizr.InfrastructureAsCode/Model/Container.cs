namespace Structurizr.InfrastructureAsCode
{
    public abstract class Container : Structurizr.Container
    {
        public virtual void InitializeUsings()
        {
        }

        public ContainerInfrastructure Infrastructure { get; set; }
    }

    public class Container<TInfrastructure> : Container where TInfrastructure : ContainerInfrastructure
    {
        public new TInfrastructure Infrastructure
        {
            get { return (TInfrastructure) base.Infrastructure; }
            set { base.Infrastructure = value; }
        }
    }
}
