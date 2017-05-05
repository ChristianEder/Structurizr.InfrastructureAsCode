using System.Collections.Generic;
using System.Linq;

namespace Structurizr.InfrastructureAsCode
{
    public static class SoftwareSystemWithInfrastructureExtensions
    {
        public static IEnumerable<ContainerWithInfrastructure> Containers(this SoftwareSystemWithInfrastructure softwareSystem)
        {
            foreach (var containerProperty in softwareSystem.GetType().GetProperties().Where(p => typeof(ContainerWithInfrastructure).IsAssignableFrom(p.PropertyType)))
            {
                var container = (ContainerWithInfrastructure)containerProperty.GetValue(softwareSystem);
                yield return container;
            }
        }
    }
}