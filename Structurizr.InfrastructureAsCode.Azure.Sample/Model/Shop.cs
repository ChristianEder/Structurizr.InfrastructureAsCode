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
            //KeyVault = new ShopKeyVault(environment);
            Database = new ShopDatabase(environment);
            Frontend = new ShopFrontend(environment, Database);
        }

        //public ShopKeyVault KeyVault { get; set; }
        public ShopFrontend Frontend { get; set; }
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
