namespace Structurizr.InfrastructureAsCode
{
    public interface IHaveInfrastructure
    {
        ContainerInfrastructure Infrastructure { get; }
    }

    public interface IHaveInfrastructure<out TInfrastructure> : IHaveInfrastructure
        where TInfrastructure : ContainerInfrastructure
    {
        new TInfrastructure Infrastructure { get; }
    }
}