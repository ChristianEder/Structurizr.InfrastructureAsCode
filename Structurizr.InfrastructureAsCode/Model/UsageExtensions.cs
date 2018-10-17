namespace Structurizr.InfrastructureAsCode.Model
{
    public static class UsageExtensions
    {
        public static Person Uses(this Person person, ContainerWithInfrastructure container, string description)
        {
            person.Uses(container.Container, description);
            person.Uses(container.Container.SoftwareSystem, description);
            return person;
        }

        public static SoftwareSystem Uses(this SoftwareSystem system, ContainerWithInfrastructure container, string description, string technology)
        {
            system.Uses(container.Container, description, technology);
            system.Uses(container.Container.SoftwareSystem, description);
            return system;
        }
    }
}