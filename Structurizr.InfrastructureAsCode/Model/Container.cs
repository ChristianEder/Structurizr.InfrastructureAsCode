namespace Structurizr.InfrastructureAsCode
{
    public abstract class Container : Structurizr.Container
    {
        public virtual void InitializeUsings()
        {
        }
    }

    public class Container<TInfrastructure> : Container where TInfrastructure : ContainerInfrastructure
    {
        public TInfrastructure Infrastructure { get; set; }
    }
}
