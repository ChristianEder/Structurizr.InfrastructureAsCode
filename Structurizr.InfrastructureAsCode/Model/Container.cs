namespace Structurizr.InfrastructureAsCode
{
    public class Container : Structurizr.Container
    {
        public ContainerInfrastructure Infrastructure { get; set; }

        public virtual void InitializeUsings()
        {
        }
    }
}
