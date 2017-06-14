using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class Shop : SoftwareSystemWithInfrastructure
    {

        public Shop(Workspace workspace, IInfrastructureEnvironment environment)
        {
            System = workspace.Model.AddSoftwareSystem("Shop", "");
            Customer = workspace.Model.AddPerson(Location.External, "Customer", "Buys stuff in our shop");

            Database = new ShopDatabase(this, environment);
            Api = new ShopApi(this, Database, environment);
            Frontend = new ShopFrontend(this, Api, environment);

            Customer.Uses(Frontend.Container, "buys stuff");

            // TODO: make implicit
            Customer.Uses(System, "buys stuff");
        }

        public ShopFrontend Frontend { get; set; }
        public ShopApi Api { get; set; }
        public ShopDatabase Database { get; set; }
        public Person Customer { get; set; }
    }
}
