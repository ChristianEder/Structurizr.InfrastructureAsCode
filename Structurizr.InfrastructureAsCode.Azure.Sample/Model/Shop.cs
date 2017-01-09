namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class Shop : SoftwareSystem
    {
        public Shop()
        {
            Name = "Shop";
            KeyVault = new ShopKeyVault();
            Database = new ShopDatabase();
            Frontend = new ShopFrontend(Database, KeyVault);
        }

        public ShopKeyVault KeyVault { get; set; }
        public ShopFrontend Frontend { get; set; }
        public ShopDatabase Database { get; set; }
    }
}
