using System.Linq;

namespace Structurizr.InfrastructureAsCode
{
    public static class SoftwareSystemExtensions
    {
        public static void AddAllContainers(this SoftwareSystem softwareSystem)
        {
            foreach (var containerProperty in softwareSystem.GetType().GetProperties().Where(p => typeof(Structurizr.Container).IsAssignableFrom(p.PropertyType)))
            {
                var container = (Structurizr.Container)containerProperty.GetValue(softwareSystem);
                softwareSystem.AddContainer(container);
            }
        }
    }
}