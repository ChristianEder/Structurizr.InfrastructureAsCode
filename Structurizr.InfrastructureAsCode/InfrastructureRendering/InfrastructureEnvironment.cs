namespace Structurizr.InfrastructureAsCode.InfrastructureRendering
{
    public class InfrastructureEnvironment : IInfrastructureEnvironment
    {
        public InfrastructureEnvironment(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}