namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class SubInfrastructure<T> : IHaveInfrastructure<T> where T : ContainerInfrastructure
    {
        public SubInfrastructure(T infrastructure)
        {
            Infrastructure = infrastructure;
        }

        public T Infrastructure { get; }

        ContainerInfrastructure IHaveInfrastructure.Infrastructure => Infrastructure;
    }
}