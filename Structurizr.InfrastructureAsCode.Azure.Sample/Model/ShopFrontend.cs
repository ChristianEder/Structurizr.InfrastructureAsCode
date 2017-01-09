using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopFrontend : Container
    {
        private readonly ShopDatabase _database;
        private readonly ShopKeyVault _keyVault;

        public ShopFrontend(ShopDatabase database, ShopKeyVault keyVault)
        {
            _database = database;
            _keyVault = keyVault;
            Name = "Shop Frontend";
            Description = "Allows the user to browse and order products";
            Technology = "ASP.NET Core MVC Web Application";

            Infrastructure = new AppService(e => $"shop-{e.Name}");
        }

        public override void InitializeUsings()
        {
            base.InitializeUsings();
            Uses(_database, "Stores and loads products and orders");
            Uses(_keyVault, "Loads access secrets to the shop database");
        }
    }
}