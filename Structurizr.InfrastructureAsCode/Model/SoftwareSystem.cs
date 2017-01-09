using System.Linq;

namespace Structurizr.InfrastructureAsCode
{
    public class SoftwareSystem : Structurizr.SoftwareSystem
    {
        public virtual void Initialize()
        {
            this.AddAllContainers();
            foreach (var container in Containers.OfType<Container>())
            {
                container.InitializeUsings();
            }
        }
    }
}