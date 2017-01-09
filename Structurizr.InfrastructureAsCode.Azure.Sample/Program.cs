using Structurizr.InfrastructureAsCode.Azure.Sample.Model;

namespace Structurizr.InfrastructureAsCode.Azure.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var workspace = ArchitectureModel();
        }

        private static Workspace ArchitectureModel()
        {
            var workspace = new Workspace("Shop architecture", "Some generic web implemented with Azure cloud infrastructure");

            var shop = new Shop();
            workspace.Model.Add(shop);
            shop.Initialize();

            var containerView = workspace.Views.CreateContainerView(shop, "Shop Container View", "Overview over the shop system");
            containerView.AddAllContainers();
            return workspace;
        }
    }
}
