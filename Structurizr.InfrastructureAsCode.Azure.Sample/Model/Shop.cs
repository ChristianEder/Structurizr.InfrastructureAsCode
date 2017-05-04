using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class Shop : SoftwareSystem
    {
        public Shop() : this(null)
        {
        }

        public Shop(IInfrastructureEnvironment environment)
        {
            Name = "Shop";
            Database = new ShopDatabase(environment);
            Api = new ShopApi(environment, Database);
            Frontend = new ShopFrontend(environment, Api);
        }

        public ShopFrontend Frontend { get; set; }
        public ShopApi Api { get; set; }
        public ShopDatabase Database { get; set; }
        public Person Customer { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            Customer = Model.AddPerson(Location.External, "Customer", "Buys stuff in our shop");
            Customer.Uses(Frontend, "buys stuff");
            Customer.Uses(this, "buys stuff");
        }
    }
}
