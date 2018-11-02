namespace Structurizr.InfrastructureAsCode.Azure.Tests.Data.Infrastructure
{
    public class MockIHaveInfrastructure : IHaveInfrastructure
    {
        public ContainerInfrastructure Infrastructure { get; set; }
    }
}