namespace Structurizr.InfrastructureAsCode
{
    public interface IHaveInfrastructure
    {
        ContainerInfrastructure Infrastructure { get; }
    }

    public interface IHaveInfrastructure<out TInfrastructure>
        where TInfrastructure : ContainerInfrastructure
    {
        TInfrastructure Infrastructure { get; }
    }
}